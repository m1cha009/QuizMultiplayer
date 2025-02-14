using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Quiz
{
	public class GamePlayManager : NetworkSingleton<GamePlayManager>, IGameplayBaseEvents, IGameplayLifecycleEvents
	{
		[SerializeField] private QuestionPoolSo _questionsPool;
		[SerializeField] private bool _useWebQuestions;
		[SerializeField] private GameplayScreen _gameplayScreen;
		[SerializeField] private EndRoundScreen _endRoundObject;
		[SerializeField] private float _gameplayTimerDuration;
		[SerializeField] private float _endRoundTimerDuration;

		public event Action<int> OnTimeChanged;
		
		private InnerScreensType _currentInnerScreen = InnerScreensType.None;
		private readonly float _syncInterval = 1f;
		private float _lastSyncTime;
		private float _localTimeLeft;
		private bool _isGameplayStarted;
		private readonly QuestionsService _questionsService = new();
		private List<Question> _currentQuestionsData = new();
		private int _totalQuestions;
		private int _questionIndex;

		private readonly Dictionary<string, string> _playersAnswersDic = new();
		private Dictionary<string, PlayerData> _playerDataDic;

		private string GetQuestion(int questionIndex) => _currentQuestionsData[questionIndex].question;
		public List<string> GetCorrectAnswers(int questionIndex) => _currentQuestionsData[questionIndex].answers.ToList();

		private int GetMaxAnswerPoints(int questionIndex) =>
			_currentQuestionsData[questionIndex].points;

		private void OnEnable()
		{
			GameplayEventDispatcher.Instance.RegisterGameplayEvents(this);
		}

		private void Update()
		{
			if (!_isGameplayStarted || !IsHost) return;
			
			if (_localTimeLeft > 0)
			{
				_localTimeLeft -= Time.deltaTime;
			
				if (Time.time - _lastSyncTime >= _syncInterval)
				{
					_lastSyncTime = Time.time;
					TimeChangedRpc(Mathf.FloorToInt(_localTimeLeft));
				}
			}
			else
			{
				var newInnerScreen = _currentInnerScreen == InnerScreensType.Gameplay
					? InnerScreensType.EndRound
					: InnerScreensType.Gameplay;
				
				ChangeInnerScreen(newInnerScreen);
			}
		}

		public void OnGameplayStarted()
		{
			_playerDataDic = GameManager.Instance.GetPlayersData();
			
			if (!IsHost) return;

			_isGameplayStarted = true;
			
			ChangeInnerScreen(InnerScreensType.Gameplay);
		}

		public void OnGameplayStopped()
		{
			if (!IsHost) return;
			
			_isGameplayStarted = false;
		}
		
		[Rpc(SendTo.ClientsAndHost)]
		public void SetPlayerAnswerRpc(string playerId, string answer)
		{
			if (!_playersAnswersDic.TryAdd(playerId, answer))
			{
				_playersAnswersDic.Remove(playerId);
				_playersAnswersDic.Add(playerId, answer);
			}
		}

		public async UniTask SetQuestions()
		{
			if (!IsHost) return;
			
			if (_useWebQuestions)
			{
				var questionData = await _questionsService.GetQuestionData();
				
				_currentQuestionsData = questionData.questions.ToList();
			}
			else
			{
				_currentQuestionsData = _questionsPool.QuestionsList;
			}

			_totalQuestions = _currentQuestionsData.Count;
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void TimeChangedRpc(int newTime)
		{
			OnTimeChanged?.Invoke(newTime);
		}

		private void ChangeInnerScreen(InnerScreensType newInnerScreen)
		{
			DisableCurrentScreenRpc(_currentInnerScreen);
			
			switch (newInnerScreen)
			{
				case InnerScreensType.None:
					break;
				
				case InnerScreensType.Gameplay:
					_localTimeLeft = _gameplayTimerDuration;
					GameManager.Instance.ClearPLayerDataAnswers();
					
					if (_questionIndex >= _totalQuestions)
					{
						GameManager.Instance.ChangeScreenRpc(ScreensType.FinishScreen);
						_questionIndex = 0;
					}
					else
					{
						var question = GetQuestion(_questionIndex);
						
						SetupGameplayScreenRpc(_questionIndex, _totalQuestions, question);
					}
					break;
				
				case InnerScreensType.EndRound:
					_localTimeLeft = _endRoundTimerDuration;
					
					if (IsHost)
					{
						AnswerCalculation();
						
						SetupEndRoundRpc(_playerDataDic.Values.ToArray());
					}
					
					_questionIndex++;
					
					break;
			}

			_currentInnerScreen = newInnerScreen;
			_lastSyncTime = 0;
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void SetupGameplayScreenRpc(int questionIndex, int totalQuestions, FixedString128Bytes question)
		{
			_playersAnswersDic.Clear();
			
			_gameplayScreen.SetupGameplayScreen(questionIndex, totalQuestions, question.Value);
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void SetupEndRoundRpc(PlayerData[] playerDataArray)
		{
			_endRoundObject.SetupEndRoundScreen(playerDataArray);
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void DisableCurrentScreenRpc(InnerScreensType currentScreen)
		{
			var currentScreenGameObj = GetCurrentScreenGameObject(currentScreen);
			if (currentScreenGameObj == null) return;
			currentScreenGameObj.SetActive(false);
		}

		private GameObject GetCurrentScreenGameObject(InnerScreensType innerScreensType)
		{
			switch (innerScreensType)
			{
				case InnerScreensType.None:
					return null;
				case InnerScreensType.Gameplay:
					return _gameplayScreen.gameObject;
				case InnerScreensType.EndRound:
					return _endRoundObject.gameObject;
			}

			return null;
		}

		private void AnswerCalculation()
		{
			var correctAnswers = GetCorrectAnswers(_questionIndex);
			var maxAnswerPoints = GetMaxAnswerPoints(_questionIndex);

			var n = 1;
			foreach (var player in _playersAnswersDic)
			{
				var playerId = player.Key;
				var playerAnswer = player.Value;
				
				_playerDataDic.TryGetValue(playerId, out var playerData);
				if (playerData == null) continue;

				var isFound = correctAnswers.Contains(playerAnswer);

				if (!isFound)
				{
					playerData.AnswerPoints = 0;
					
					continue;
				}

				var answerPoints = (int)(maxAnswerPoints * Math.Exp(-0.5f * (n - 1)));
				playerData.AnswerPoints = answerPoints;

				switch (playerData.SkillType)
				{
					case SkillType.None:
						playerData.SkillPoints = answerPoints;
						break;
					case SkillType.X2:
						playerData.SkillPoints = answerPoints * 2;
						break;
					case SkillType.Resist:
						playerData.SkillPoints = answerPoints;
						break;
				}
				
				UpdateTotalPointsRpc(playerId, playerData.SkillPoints);
				
				n++;
			}
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void UpdateTotalPointsRpc(string playerId, int answerPoints)
		{
			if (_playerDataDic.TryGetValue(playerId, out var playerData))
			{
				playerData.TotalPoints += answerPoints;
			}
		}
		
	}
}