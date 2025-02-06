using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace Quiz
{
	public class GameManager : NetworkSingleton<GameManager>, ISessionProvider, IBaseSession, ISessionLifecycleEvents
	{
		[SerializeField] private ScreensType _defaultScreen;
		
		[SerializeField] private LobbyScreen _lobbyScreen;
		[SerializeField] private GameScreen _gameScreen;
		
		private GameScreenFactory _currentScreen;
		
		public ISession Session { get; set; }
		public string CurrentPlayerId => Session.CurrentPlayer.Id;
		
		private void OnEnable()
		{
			SessionEventsDispatcher.Instance.RegisterBaseClassEvents(this);
		}
		
		private void Start()
		{
			_currentScreen = GetScreen(_defaultScreen);
			_currentScreen.Enable();
		}
		
		[Rpc(SendTo.ClientsAndHost)]
		public void ChangeScreenRpc(ScreensType screen)
		{
			_currentScreen.Disable();

			switch (screen)
			{
				case ScreensType.None:
					break;
				case ScreensType.Lobby:
					_lobbyScreen.Enable();
					_currentScreen = _lobbyScreen;
					break;
				case ScreensType.GamePlay:
					_gameScreen.Enable();
					_currentScreen = _gameScreen;
					break;
			}
		}
		
		public void OnSessionJoined()
		{
		}

		public void OnSessionLeft()
		{
			SystemLogger.Log("Host left server. Returning to lobby screen");
			Debug.Log("Host left server. Returning to lobby screen");
			
			_currentScreen.Disable();
			_lobbyScreen.Enable();
			_currentScreen = _lobbyScreen;
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

		private GameScreenFactory GetScreen(ScreensType screen)
		{
			switch (screen)
			{
				case ScreensType.None:
					break;
				
				case ScreensType.Lobby:
					return _lobbyScreen;
				
				case ScreensType.GamePlay:
					return _gameScreen;
			}

			return null;
		}


	}
}