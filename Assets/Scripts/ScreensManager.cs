using Unity.Netcode;
using UnityEngine;

namespace Quiz
{
	public class ScreensManager : NetworkSingleton<ScreensManager>
	{
		[SerializeField] private LobbyScreen _lobbyScreen;
		[SerializeField] private GameScreen _gameScreen;

		private GameScreenFactory _currentScreen;

		private void Start()
		{
			_currentScreen = _lobbyScreen;
			
			_lobbyScreen.Enable();
			_gameScreen.Disable();
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
	}
}