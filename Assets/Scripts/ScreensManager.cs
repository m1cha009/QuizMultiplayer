using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace DefaultNamespace
{

	public class ScreensManager : NetworkBehaviour
	{
		public enum ScreensEnum
		{
			ConnectionScreen,
			LobbyScreen,
		}
		
		[SerializeField] private ConnectionManager _connectionManager;
		[SerializeField] private LobbyScreen _lobbyScreen;
		[SerializeField] private TMP_Text _errorText;
		
		public static ScreensManager Instance { get; private set; }
		
		private ulong _localClientId;

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


		public void DisconnectPlayerRpc()
		{
			if (HasAuthority)
			{
				_errorText.SetText($"Player {_localClientId} disconnected");
				NetworkManager.Singleton.DisconnectClient(_localClientId);
				NetworkManager.Singleton.Shutdown();
			}
		}

		private void SingletonOnOnClientConnectedCallback(ulong clientId)
		{
			if (NetworkManager.Singleton.LocalClientId == clientId)
			{
				Debug.Log($"My id is {clientId}");
			}
			
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