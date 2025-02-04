using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace Quiz
{
	public struct PlayerData : INetworkSerializeByMemcpy, IEquatable<PlayerData>
	{
		public FixedString32Bytes PlayerId;
		public bool IsReady;

		public bool Equals(PlayerData other)
		{
			return PlayerId == other.PlayerId && IsReady == other.IsReady;
		}

		public override bool Equals(object obj)
		{
			return obj is PlayerData other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(PlayerId, IsReady);
		}
	}
	
	public class SessionPlayerList : NetworkBaseSession, ISessionProvider, ISessionEvents, ISessionLifecycleEvents
	{
		[SerializeField] private SessionPlayerItem _sessionPlayerItemPrefab;
		[SerializeField] private Transform _parentTransform;
		
		private readonly Dictionary<string, SessionPlayerItem> _sessionPlayerItems = new();
		private NetworkList<PlayerData> _playersReadyState;

		public ISession Session { get; set; }

		private void Awake()
		{
			_playersReadyState = new NetworkList<PlayerData>();
		}

		public override void OnNetworkSpawn()
		{
			UpdateReadyList();
		}

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
			if (_sessionPlayerItems.TryGetValue(playerId, out var sessionPlayerItem))
			{
				sessionPlayerItem.SetReady(ready);

				if (Session.IsHost)
				{
					_playersReadyState.Add(new PlayerData {PlayerId = playerId, IsReady = ready});
				}
			}
		}

		public void OnSessionDeleted()
		{
			Debug.Log("OnSessionDeleted");
			SystemLogger.Log("OnSessionDeleted");
			
			RemoveAllPlayerList();
		}

		public void OnSessionJoined()
		{
			UpdatePlayerList();
		}

		public void OnSessionLeft()
		{
			RemoveAllPlayerList();
		}

		private void UpdateReadyList()
		{
			if (Session == null) return;
			
			foreach (var player in Session.Players)
			{
				var playerId = player.Id;

				if (!_sessionPlayerItems.TryGetValue(playerId, out var sessionPlayerItem))
				{
					Debug.Log($"Player {playerId} not found");
					
					return;
				}
				
				foreach (var playerData in _playersReadyState)
				{
					if (playerData.PlayerId.Value == playerId)
					{
						sessionPlayerItem.SetReady(playerData.IsReady);
					}
				}
			}
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