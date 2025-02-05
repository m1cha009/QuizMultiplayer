using System.Collections.Generic;
using Quiz.SO;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace Quiz
{
	public interface IGameplayBaseEvents
	{
	}

	public interface IGameplayLifecycleEvents
	{
		void OnGameplayStarted();
		void OnGameplayStopped();
	}

	public class GameplayManager : NetworkSingleton<GameplayManager>, ISessionProvider, IBaseSession
	{
		[SerializeField] private QuestionPoolSo _questionsPool;

		public ISession Session { get; set; }
		public string CurrentPlayerId => Session.CurrentPlayer.Id;
		public int TotalQuestionsAmount => _questionsPool.QuestionPool.Count;
		
		private void OnEnable()
		{
			SessionEventsDispatcher.Instance.RegisterBaseClassEvents(this);
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
		
		public string GetQuestion(int questionId) => _questionsPool.QuestionPool[questionId].Question;
	}
}