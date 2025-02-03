using Unity.Netcode;
using UnityEngine;

namespace Quiz
{
	public class ScreensManager : NetworkSingleton<ScreensManager>
	{
		[SerializeField] private GameObject _lobbyScreen;
		[SerializeField] private GameObject _gameScreen;

		private GameObject _currentScreen;

		private void Start()
		{
			_currentScreen = _lobbyScreen;
			
			_gameScreen.SetActive(false);
			
			Debug.Log($"ScreenManager: {Instance}");
		}



		[Rpc(SendTo.ClientsAndHost)]
		public void ChangeScreenRpc(ScreensType screen)
		{
			_currentScreen.SetActive(false);

			switch (screen)
			{
				case ScreensType.None:
					break;
				case ScreensType.Lobby:
					_lobbyScreen.SetActive(true);
					_currentScreen = _lobbyScreen;
					break;
				case ScreensType.Game:
					_gameScreen.SetActive(true);
					_currentScreen = _gameScreen;
					break;
			}
		}
	}
}