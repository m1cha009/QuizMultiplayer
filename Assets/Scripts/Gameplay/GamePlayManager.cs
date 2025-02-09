using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Quiz
{
	public class GamePlayManager : NetworkSingleton<GamePlayManager>, IGameplayBaseEvents, IGameplayLifecycleEvents
	{
		[SerializeField] private QuestionPoolSo _questionsPool;
		[SerializeField] private GameplayScreen _gameplayScreen;
		[SerializeField] private EndRoundScreen _endRoundObject;

		public event Action<int> OnTimeChanged;
		
		private InnerScreensType _currentInnerScreen = InnerScreensType.Gameplay;
		private readonly float _syncInterval = 1f;
		private float _lastSyncTime;
		private float _localTimeLeft;
		private bool _isGameplayStarted;

		private readonly float _gameplayTimerDuration = 7;
		private readonly float _endRoundTimerDuration = 5;
		private readonly Dictionary<string, string> _playersAnswersDic = new();
		
		public int TotalQuestionsAmount => _questionsPool.QuestionPool.Count;
		public int QuestionIndex { get; private set; }
		public string GetQuestion(int questionIndex) => _questionsPool.QuestionPool[questionIndex].Question;
		public List<string> GetCorrectAnswers(int questionIndex) => _questionsPool.QuestionPool[questionIndex].CorrectAnswers;

		public int GetMaxAnswerPoints(int questionIndex) =>
			_questionsPool.QuestionPool[questionIndex].CorrectAnswerPoints;

		private void OnEnable()
		{
			GameplayEventDispatcher.Instance.RegisterGameplayEvents(this);
		}

		private void Update()
		{
			if (!_isGameplayStarted || !IsHost) return;

			if (QuestionIndex >= TotalQuestionsAmount)
			{
				GameManager.Instance.ChangeScreenRpc(ScreensType.FinishScreen);
				QuestionIndex = 0;
				
				return;
			}
			
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
				
				SetupInnerScreen(newInnerScreen);
				ChangeInnerScreensRpc(newInnerScreen);
			}
		}
		
		public void OnGameplayStarted()
		{
			_isGameplayStarted = true;
			
			SetupInnerScreen(_currentInnerScreen);
		}

		public void OnGameplayStopped()
		{
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

		[Rpc(SendTo.ClientsAndHost)]
		private void TimeChangedRpc(int newTime)
		{
			OnTimeChanged?.Invoke(newTime);
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void ChangeInnerScreensRpc(InnerScreensType newInnerScreen)
		{
			var currentScreenGameObj = GetCurrentScreenGameObject(_currentInnerScreen);
			currentScreenGameObj.SetActive(false);
			
			switch (newInnerScreen)
			{
				case InnerScreensType.None:
					break;
				case InnerScreensType.Gameplay:
					_gameplayScreen.SetupGameplayScreen(QuestionIndex);
					_gameplayScreen.gameObject.SetActive(true);
					break;
				case InnerScreensType.EndRound:
					AnswerCalculation();
					
					QuestionIndex++;
					
					var playerData = GameManager.Instance.GetPlayersData();
					
					_endRoundObject.SetupEndRoundScreen(playerData.Values.ToList());
					_endRoundObject.gameObject.SetActive(true);
					break;
			}

			_currentInnerScreen = newInnerScreen;
		}

		private void SetupInnerScreen(InnerScreensType innerScreensType)
		{
			switch (innerScreensType)
			{
				case InnerScreensType.None:
					break;
				case InnerScreensType.Gameplay:
					_localTimeLeft = _gameplayTimerDuration;
					
					break;
				case InnerScreensType.EndRound:
					_localTimeLeft = _endRoundTimerDuration;
					
					break;
			}

			_lastSyncTime = 0;
		}

		private GameObject GetCurrentScreenGameObject(InnerScreensType innerScreensType)
		{
			switch (innerScreensType)
			{
				case InnerScreensType.None:
					break;
				case InnerScreensType.Gameplay:
					return _gameplayScreen.gameObject;
				case InnerScreensType.EndRound:
					return _endRoundObject.gameObject;
			}

			return null;
		}

		private void AnswerCalculation()
		{
			var correctAnswers = GetCorrectAnswers(QuestionIndex);
			var playersData = GameManager.Instance.GetPlayersData();
			var maxAnswerPoints = GetMaxAnswerPoints(QuestionIndex);

			var n = 1;
			foreach (var player in _playersAnswersDic)
			{
				var playerId = player.Key;
				var playerAnswer = player.Value;
				
				playersData.TryGetValue(playerId, out var playerData);
				if (playerData == null) continue;
				
				var isFound = correctAnswers.Contains(playerAnswer);

				if (!isFound)
				{
					playerData.AnswerPoints = 0;
					
					continue;
				}
				
				playerData.AnswerPoints = (int)(maxAnswerPoints * Math.Exp(-0.5f * (n - 1)));
				playerData.TotalPoints += playerData.AnswerPoints;
				
				n++;
			}
		}
		
	}
}