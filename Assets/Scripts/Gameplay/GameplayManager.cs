using System;
using System.Collections.Generic;
using Quiz.SO;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace Quiz
{
	public interface IGameplayBaseEvents
	{
	}

	public interface IGameplayLifecycleEvents
	{
		void OnGameplayInitialized();
		void OnGameplayDeInitialized();
	}

	public class GameplayManager : NetworkSingleton<GameplayManager>, ISessionProvider, IBaseSession
	{
		[SerializeField] private PlayerListPanel _playerListPanel;
		[SerializeField] private QuestionsPanel _questionsPanel;
		[SerializeField] private QuestionPoolSo _questionsPool;


		private readonly int _countdownDuration = 5;
		private readonly NetworkVariable<int> _serverTimeLeft = new ();
		private readonly float _syncInterval = 1f;
		private float _lastSyncTime;
		private float _localTimeLeft;

		public event Action OnQuestionTimeElapsed;
		
		
		public bool TimerInitialized { get; set; }

		public ISession Session { get; set; }
		public string CurrentPlayerId => Session.CurrentPlayer.Id;
		public int TotalQuestionsAmount => _questionsPool.QuestionPool.Count;
		
		private void OnEnable()
		{
			SessionEventsDispatcher.Instance.RegisterBaseClassEvents(this);
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();
			
			// GameplayEventDispatcher.Instance.OnGameplayInitialized();

			if (IsServer)
			{
				_serverTimeLeft.Value = _countdownDuration;
			}
			
			_localTimeLeft = _countdownDuration;
			_serverTimeLeft.OnValueChanged += OnTimerValueChanged;
		}

		public override void OnNetworkDespawn()
		{
			// GameplayEventDispatcher.Instance.OnGameplayDeInitialized();
			
			_serverTimeLeft.OnValueChanged -= OnTimerValueChanged;
			TimerInitialized = false;
			
			base.OnNetworkDespawn();
		}

		private void OnTimerValueChanged(int previousValue, int newValue)
		{
			// _questionsPanel.SetTimer(newValue);
		}

		private void Update()
		{
			if (!IsServer || !TimerInitialized) return;

			CalculateQuestionTimeLeft();
		}
		
		public List<PlayerData> GetPlayersData()
		{
			if (Session == null)
			{
				SystemLogger.Log("Session is null");
				Debug.Log("Session is null");
				return null;
			}

			var playersData = new List<PlayerData>();
			foreach (var player in Session.Players)
			{
				var playerId = player.Id;
				var playerName = "UnKnown";
				if (player.Properties.TryGetValue(SessionConstants.PlayerNameProperty, out var playerProperty))
				{
					playerName = playerProperty.Value;
				}

				var playerData = new PlayerData { PlayerId = playerId, PlayerName = playerName, Answer = string.Empty };
				playersData.Add(playerData);
			}

			return playersData;
		}

		[Rpc(SendTo.ClientsAndHost)]
		public void SetAnswerRpc(string playerId, string answer)
		{
			_playerListPanel.SetPlayerAnswerRpc(playerId, answer);
		}

		private void CalculateQuestionTimeLeft()
		{
			if (_localTimeLeft > 0)
			{
				_localTimeLeft -= Time.deltaTime;
			
				if (Time.time - _lastSyncTime >= _syncInterval)
				{
					_lastSyncTime = Time.time;
					_serverTimeLeft.Value = Mathf.FloorToInt(_localTimeLeft); // it is going to call OnTimerValueChanged
				}
			}
			else
			{
				OnQuestionTimeElapsed?.Invoke();
			}
		}
		
		[Rpc(SendTo.ClientsAndHost)]
		public void SetupNextQuestionRpc(int questionIndex)
		{
			var question = _questionsPool.QuestionPool[questionIndex].Question;
			
			_questionsPanel.DisplayQuestion(questionIndex, question);
			_localTimeLeft = _countdownDuration;
		}


	}
}