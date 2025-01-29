using System.Collections.Generic;
using Quiz.Constants;
using Quiz.Interfaces;
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
			
			Debug.Log("On Player joined");
		}

		public void OnSessionJoined()
		{
			UpdatePlayerList();
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

		
	}
}