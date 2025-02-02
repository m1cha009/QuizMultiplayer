using System.Collections.Generic;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace Quiz
{
	public class SessionPlayerList : BaseSession, ISessionProvider, ISessionEvents, ISessionLifecycleEvents
	{
		[SerializeField] private SessionPlayerItem _sessionPlayerItemPrefab;
		[SerializeField] private Transform _parentTransform;
		
		private readonly Dictionary<string, SessionPlayerItem> _sessionPlayerItems = new();
		
		public ISession Session { get; set; }

		public void OnPlayerJoined(string playerId)
		{
			UpdatePlayerList();
			
			var playerName = string.Empty;
			if (_sessionPlayerItems.TryGetValue(playerId, out var sessionPlayerItem))
			{
				playerName = sessionPlayerItem.PlayerName;
			}
			
			SystemLogger.Log($"Player {playerName} joined");
			Debug.Log($"Player {playerName} joined");
		}

		public void OnPlayerLeft(string playerId)
		{
			var playerName = string.Empty;
			if (_sessionPlayerItems.TryGetValue(playerId, out var sessionPlayerItem))
			{
				playerName = sessionPlayerItem.PlayerName;
			}
			
			SystemLogger.Log($"Player {playerName} left");
			Debug.Log($"Player {playerName} left");
			
			RemovePLayer(playerId);
		}

		public void OnPlayerReadyTrigger(string playerId, bool ready)
		{
			// TODO: display that player pressed Ready
		}

		public void OnSessionJoined()
		{
			UpdatePlayerList();
		}

		public void OnSessionLeft()
		{
			RemoveAllPlayerList();
		}

		private void UpdatePlayerList()
		{
			if (Session == null) return;

			foreach (var player in Session.Players)
			{
				var playerId = player.Id;
				
				if (_sessionPlayerItems.ContainsKey(playerId))
				{
					continue;
				}

				var playerName = "Somebody";
				if (player.Properties.TryGetValue(SessionConstants.PlayerNameProperty, out var playerProperty))
				{
					playerName = playerProperty.Value;
				}

				var playerItem = Instantiate(_sessionPlayerItemPrefab, _parentTransform);
				
				playerItem.Init(playerId, playerName);
				
				_sessionPlayerItems.Add(playerId, playerItem);
			}
		}

		private void RemovePLayer(string playerId)
		{
			if (_sessionPlayerItems.ContainsKey(playerId))
			{
				Destroy(_sessionPlayerItems[playerId].gameObject);
				
				_sessionPlayerItems.Remove(playerId);
			}
		}

		private void RemoveAllPlayerList()
		{
			foreach (var sessionPlayerItem in _sessionPlayerItems)
			{
				Destroy(sessionPlayerItem.Value.gameObject);
			}
			
			_sessionPlayerItems.Clear();
		}
	}
}