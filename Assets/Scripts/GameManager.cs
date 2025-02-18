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
		[SerializeField] private FinishScreenManager _finishScreen;
		
		private BaseScreens _currentBaseScreen;
		private readonly Dictionary<string, PlayerData> _playersDataDic = new();
		
		public ISession Session { get; set; }
		public string CurrentPlayerId => Session.CurrentPlayer.Id;
		
		private void OnEnable()
		{
			SessionEventsDispatcher.Instance.RegisterBaseClassEvents(this);
		}
		
		private void Start()
		{
			_currentBaseScreen = GetScreen(_defaultScreen);
			_currentBaseScreen.Enable();
		}

		public void OnSessionJoined()
		{
		}

		public void OnSessionLeft()
		{
			SystemLogger.Log("Host left server. Returning to lobby screen");
			Debug.Log("Host left server. Returning to lobby screen");
			
			_currentBaseScreen.Disable();
			_lobbyScreen.Enable();
			_currentBaseScreen = _lobbyScreen;
		}

		[Rpc(SendTo.ClientsAndHost)]
		public void ChangeScreenRpc(ScreensType screen)
		{
			ChangeScreen(screen);
		}
		
		public void ChangeScreen(ScreensType screen)
		{
			_currentBaseScreen.Disable();

			switch (screen)
			{
				case ScreensType.None:
					break;
				case ScreensType.Lobby:
					_lobbyScreen.Enable();
					_currentBaseScreen = _lobbyScreen;
					break;
				case ScreensType.GamePlay:
					_gameScreen.Enable();
					_currentBaseScreen = _gameScreen;
					break;
				case ScreensType.FinishScreen:
					_finishScreen.Enable();
					_currentBaseScreen = _finishScreen;
					break;
			}
		}

		public void InitializePlayersData()
		{
			if (Session == null)
			{
				SystemLogger.Log("Session is null");
				Debug.Log("Session is null");
				return;
			}

			foreach (var player in Session.Players)
			{
				var playerId = player.Id;

				if (_playersDataDic.ContainsKey(playerId))
				{
					continue;
				}
				
				var playerName = "UnKnown";
				if (player.Properties.TryGetValue(SessionConstants.PlayerNameProperty, out var playerProperty))
				{
					playerName = playerProperty.Value;
				}

				var playerData = new PlayerData
				{
					PlayerId = playerId, 
					PlayerName = playerName, 
					Answer = string.Empty,
					TotalPoints = 1000
				};
				
				_playersDataDic.Add(playerId, playerData);
			}
		}
		
		public Dictionary<string, PlayerData> GetPlayersData() => _playersDataDic;
		
		public PlayerData GetPlayerData(string playerId) => _playersDataDic[playerId];

		public void ClearPLayerDataAnswers()
		{
			foreach (var playerData in _playersDataDic.Values)
			{
				playerData.Answer = string.Empty;
				playerData.AnswerPoints = 0;
				
				for (var i = 0; i < playerData.SkillTypes.Length; i++)
				{
					playerData.SkillTypes[i] = SkillType.None;
				}

				playerData.SkillIndex = 0;
				playerData.SkillPoints = 0;
				playerData.SkillPrice = 0;
			}
		}
		
		private BaseScreens GetScreen(ScreensType screen)
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