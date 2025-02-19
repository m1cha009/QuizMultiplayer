using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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

		private readonly OrderedDictionary _orderedAnswersDic = new();
		private Dictionary<string, Player> _playersDic;

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
			_playersDic = GameManager.Instance.GetPlayersDictionary();
			
			if (!IsHost) return;

			_isGameplayStarted = true;
			
			ChangeInnerScreen(InnerScreensType.Gameplay);
		}

		public void OnGameplayStopped()
		{
			if (!IsHost) return;
			
			_isGameplayStarted = false;
		}
		
		[Rpc(SendTo.Server)]
		public void AddOrderedAnswerRpc(string playerId, string answer)
		{
			if (!_orderedAnswersDic.Contains(playerId))
			{
				Debug.LogError($"Player {playerId} doesn't exist");
				return;
			}
			
			_orderedAnswersDic.Remove(playerId);
			_orderedAnswersDic.Add(playerId, answer);
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
					GameManager.Instance.ClearPlayerData();
					
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

						var networkPlayersDataArray = new NetworkPlayerData[_playersDic.Values.Count];
						
						var n = 0;
						foreach (var player in _playersDic.Values)
						{
							networkPlayersDataArray[n] = new NetworkPlayerData(player.PlayerData);
							n++;
						}

						SetupEndRoundRpc(networkPlayersDataArray);
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
			if (IsHost)
			{
				_orderedAnswersDic.Clear();
			
				foreach (var playerId in _playersDic.Keys)
				{
					_orderedAnswersDic.Add(playerId, string.Empty);
				}
			}
			
			_gameplayScreen.SetupGameplayScreen(questionIndex, totalQuestions, question.Value);
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void SetupEndRoundRpc(NetworkPlayerData[] networkPLayerDataArray)
		{
			var playerDataList = new List<PlayerData>();
			foreach (var networkPlayerData in networkPLayerDataArray)
			{
				var playerData = new PlayerData(networkPlayerData);
				playerDataList.Add(playerData);
			}
			
			_endRoundObject.SetupEndRoundScreen(playerDataList);
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
			foreach (DictionaryEntry answer in _orderedAnswersDic)
			{
				var playerId = answer.Key.ToString();
				var playerAnswer = answer.Value.ToString();
				
				_playersDic.TryGetValue(playerId, out var player);
				if (player == null) continue;
				
				var playerData = player.PlayerData;
				var playerSkillsData = player.PlayerData.PlayerSkillsData;

				var isFound = correctAnswers.Contains(playerAnswer);
				// var totalPoints = -playerData.SkillPrice;
				
				var totalPoints = 0;

				if (!isFound)
				{
					playerData.AnswerPoints = 0;
					playerData.SkillPoints = totalPoints;
				}
				else
				{
					var correctAnswerPoints = (int)(maxAnswerPoints * Math.Exp(-0.5f * (n - 1)));
					playerData.AnswerPoints = correctAnswerPoints;
					
					totalPoints += correctAnswerPoints;

					foreach (var skillType in playerSkillsData)
					{
						switch (skillType.SkillType)
						{
							case SkillType.None:
								continue;
							case SkillType.X2:
								totalPoints *= 2;
								break;
							case SkillType.Resist:
								break;
							case SkillType.Reverse:
								totalPoints *= -1;
								break;
						}
						
						playerData.SkillPoints = totalPoints;
					}
					
					n++;
				}
				
				UpdateTotalPointsRpc(playerId, totalPoints);
			}
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void UpdateTotalPointsRpc(string playerId, int pointsGained)
		{
			if (_playersDic.TryGetValue(playerId, out var player))
			{
				var playerData = player.PlayerData;
				
				if (playerData.TotalPoints + pointsGained < 0)
				{
					playerData.TotalPoints = 0;
				}
				else
				{
					playerData.TotalPoints += pointsGained;
				}
			}
		}
		
	}
}