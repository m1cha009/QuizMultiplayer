using System;
using Unity.Netcode;
using UnityEngine;

namespace DefaultNamespace
{
	public class ScreensManager : MonoBehaviour
	{
		[SerializeField] private ConnectionManager _connectionManager;
		[SerializeField] private LobbyScreen _lobbyScreen;
		
		public static ScreensManager Instance { get; private set; }

		private ScreensEnum _currentScreen;

		public enum ScreensEnum
		{
			ConnectionScreen,
			LobbyScreen
		}

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
		}

		private void Start()
		{
			NetworkManager.Singleton.OnClientConnectedCallback += SingletonOnOnClientConnectedCallback;
		}

		private void SingletonOnOnClientConnectedCallback(ulong obj)
		{
			var connectedClients = NetworkManager.Singleton.ConnectedClientsIds;
			
			_lobbyScreen.ClearPlayerNames();

			foreach (var client in connectedClients)
			{
				_lobbyScreen.SetPlayerName(client.ToString());
			}
		}

		public void ChangeToScreen(ScreensEnum screen)
		{
			switch (screen)
			{
				case ScreensEnum.ConnectionScreen:
					_connectionManager.Show();
					break;
				case ScreensEnum.LobbyScreen:
					_lobbyScreen.Show();
					break;
			}
		}
	}
}