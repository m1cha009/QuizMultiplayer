using System;
using System.Threading.Tasks;
using DefaultNamespace;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
	[SerializeField] private TMP_InputField _playerNameInputField;
	[SerializeField] private TMP_InputField _joinCodeInputField;
	[SerializeField] private TMP_Text _joinCodeText;
	[SerializeField] private TMP_Text _errorText;
	
	private string _profileName; 
	private string _joinCode; 
	private int _maxPlayers = 10; 
	private ConnectionState _state = ConnectionState.Disconnected; 
	private ISession _session;

   private enum ConnectionState
   {
	   Disconnected,
	   Connecting,
	   Connected,
   }
   
	private void Awake()
	{
		_playerNameInputField.onValueChanged.AddListener(OnPlayerNameChanged);
		_joinCodeInputField.onValueChanged.AddListener(OnSessionNameChanged);
	}

	private async void Start()
	{
		NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
		NetworkManager.Singleton.OnSessionOwnerPromoted += OnSessionOwnerPromoted;
		await UnityServices.InitializeAsync();
	}

	private void OnDestroy()
	{
		_playerNameInputField.onValueChanged.RemoveListener(OnPlayerNameChanged);
		_joinCodeInputField.onValueChanged.RemoveListener(OnSessionNameChanged);
		
		_session?.LeaveAsync();
	}
	
	
	public void StartHostGame()
	{
		if (!string.IsNullOrEmpty(_profileName))
		{
			CreateSessionAsync();

			Hide();
		}
		else
		{
			Debug.Log($"Cannot start server");
		}
	}

	public void StartClientGame()
	{
		if (IsNamesFilled())
		{
			JoinSessionAsync();
			
			Hide();
		}
		else
		{
			Debug.Log($"Cannot connect server");
		}
	}
	
	public void Show()
	{
		gameObject.SetActive(true);
	}

	private bool IsNamesFilled()
	{
		return !string.IsNullOrEmpty(_profileName) && !string.IsNullOrEmpty(_joinCode);
	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}

	private void OnSessionOwnerPromoted(ulong sessionOwnerPromoted)
	{
		if (NetworkManager.Singleton.LocalClient.IsSessionOwner)
		{
			Debug.Log($"Client-{NetworkManager.Singleton.LocalClientId} is the session owner!");
		}
	}

	private void OnClientConnectedCallback(ulong clientId)
	{
		if (NetworkManager.Singleton.LocalClientId == clientId)
		{
			Debug.Log($"Client-{clientId} is connected and can spawn {nameof(NetworkObject)}s.");
		}
	}
	
	private void OnSessionNameChanged(string sessionName)
	{
		_joinCode = sessionName;
	}

	private void OnPlayerNameChanged(string playerName)
	{
		_profileName = playerName;
	}

   private async Task CreateSessionAsync()
   {
	   _state = ConnectionState.Connecting;
	   
	   Debug.Log($"Starting server...");
	   _errorText.SetText($"Starting server...");

	   try
	   {
		   if (!AuthenticationService.Instance.IsSignedIn)
		   {
			   await AuthenticationService.Instance.SignInAnonymouslyAsync();
		   }

		   if (!AuthenticationService.Instance.SessionTokenExists)
		   {
			   AuthenticationService.Instance.SwitchProfile(_profileName);
		   }

		   var options = new SessionOptions
		   {
			   MaxPlayers = _maxPlayers
		   }.WithDistributedAuthorityNetwork();

		   _session = await MultiplayerService.Instance.CreateSessionAsync(options);

		   _joinCodeText.SetText(_session.Code);
		   
		   Debug.Log($"Connected to {_session.Code}");

		   _state = ConnectionState.Connected;
		   ScreensManager.Instance.ChangeToScreen(ScreensManager.ScreensEnum.LobbyScreen);
	   }
	   catch (AggregateException ae)
	   {
		   _state = ConnectionState.Disconnected;
		   
		   AuthenticationService.Instance.SignOut();
		   
		   Show();
		   
		   Debug.LogError($"Aggregator Exception: {ae}");
		   Debug.LogException(ae);
	   }
	   catch (Exception e)
	   {
		   Debug.LogException(e);
	   }
   }

   private async Task JoinSessionAsync()
   {
	   _state = ConnectionState.Connecting;
	   
	   Debug.Log($"Connecting to {_joinCode}...");
	   _errorText.SetText($"Connected to {_joinCode}");

	   try
	   {
		   if (!AuthenticationService.Instance.IsSignedIn)
		   {
			   await AuthenticationService.Instance.SignInAnonymouslyAsync();
		   }

		   if (!AuthenticationService.Instance.SessionTokenExists)
		   {
			   AuthenticationService.Instance.SwitchProfile(_profileName);
		   }

		   _session = await MultiplayerService.Instance.JoinSessionByCodeAsync(_joinCode);

		   Debug.Log($"Connected to {_session.Code}");
		   _errorText.SetText($"Connected to {_session.Code}");

		   _state = ConnectionState.Connected;
		   ScreensManager.Instance.ChangeToScreen(ScreensManager.ScreensEnum.LobbyScreen);
	   }
	   catch (AggregateException ae)
	   {
		   foreach (var exception in ae.InnerExceptions)
		   {
			   if (exception is SessionException sessionException)
			   {
				   if (sessionException.Error == SessionError.SessionNotFound)
				   {
					   _errorText.SetText($"Lobby with name {_joinCode} not exist");
				   }
				   else
				   {
					   Debug.Log($"Exception Error: {sessionException.Error}");
					   Debug.LogException(sessionException);
				   }
			   }
			   else
			   {
				   Debug.LogException(exception);
			   }
		   }

		   _state = ConnectionState.Disconnected;

		   AuthenticationService.Instance.SignOut();

		   Show();
	   }
	   catch (Exception e)
	   {
		   Debug.LogException(e);
	   }
   }
}
