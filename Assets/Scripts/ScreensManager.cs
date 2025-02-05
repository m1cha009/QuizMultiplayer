using Unity.Netcode;
using UnityEngine;

namespace Quiz
{
	public class ScreensManager : NetworkSingleton<ScreensManager>
	{
		[SerializeField] private LobbyScreen _lobbyScreen;
		[SerializeField] private GameScreen _gameScreen;
		
		[SerializeField] private ScreensType _defaultScreen;

		private GameScreenFactory _currentScreen;

		private void Start()
		{
			_currentScreen = LocalChangeScreenRpc(_defaultScreen);
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
				case ScreensType.Game:
					_gameScreen.Enable();
					_currentScreen = _gameScreen;
					break;
			}
		}

		private GameScreenFactory LocalChangeScreenRpc(ScreensType screen)
		{
			switch (screen)
			{
				case ScreensType.None:
					break;
				
				case ScreensType.Lobby:
					return _lobbyScreen;
				
				case ScreensType.Game:
					return _gameScreen;
			}

			return null;
		}
	}
}