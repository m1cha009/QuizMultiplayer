using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace Quiz
{
	public class SessionManager : SingletonTemplate<SessionManager>
	{
		private ISession _activeSession;
		private SessionEventsDispatcher _sessionEventsDispatcher;
		private const string playerNameProperty = "playerName";

		public string PlayerName { get; set; } = string.Empty;
		
		public ISession ActiveSession
		{
			get => _activeSession;
			set
			{
				if (value != null)
				{
					_activeSession = value;
					RegisterSessionEvents();
					_sessionEventsDispatcher.OnSessionJoined(_activeSession);
				}
				else if (_activeSession != null)
				{
					_activeSession = null;
					_sessionEventsDispatcher.OnSessionLeft();
				}
			}
		}

		private async void Start()
		{
			try
			{
				_sessionEventsDispatcher = SessionEventsDispatcher.Instance;
				
				await UnityServices.InitializeAsync(); // initialize unity gaming services
				await AuthenticationService.Instance.SignInAnonymouslyAsync();
				
				SystemLogger.Log($"Sign in anonymously. Player ID: {AuthenticationService.Instance.PlayerId}");
				Debug.Log($"Sign in anonymously. Player ID: {AuthenticationService.Instance.PlayerId}");
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		private void RegisterSessionEvents()
		{
			ActiveSession.PlayerJoined += _sessionEventsDispatcher.OnPlayerJoined;
			ActiveSession.PlayerLeft += _sessionEventsDispatcher.OnPlayerLeft;
		}

		private void UnRegisterSessionEvents()
		{
			ActiveSession.PlayerJoined -= _sessionEventsDispatcher.OnPlayerJoined;
			ActiveSession.PlayerLeft -= _sessionEventsDispatcher.OnPlayerLeft;
		}

		private Dictionary<string, PlayerProperty> GetPlayerProperties()
		{
			var playerNameProperties = new PlayerProperty(PlayerName, VisibilityPropertyOptions.Member);

			return new Dictionary<string, PlayerProperty>
			{
				{ playerNameProperty, playerNameProperties }
			};
		}

		public async UniTask StartSessionAsHost()
		{
			SystemLogger.Log("Starting session...");
			Debug.Log("Creating session...");
			var playerProperties = GetPlayerProperties();
			
			var options = new SessionOptions()
			{
				MaxPlayers = 6,
				PlayerProperties = playerProperties
			}.WithDistributedAuthorityNetwork();


			ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
			
			SystemLogger.Log($"Player {PlayerName} created session: {ActiveSession.Id}. Join Code: {ActiveSession.Code}");
			Debug.Log($"Player {PlayerName} created session: {ActiveSession.Id}. Join Code: {ActiveSession.Code}");
		}

		public async UniTask JoinSessionByJoinCode(string code)
		{
			try
			{
				SystemLogger.Log("Connecting session...");
				Debug.Log("Connecting to session...");

				var playerProperties = GetPlayerProperties();

				var joinSessionOptions = new JoinSessionOptions()
				{
					PlayerProperties = playerProperties
				};

				ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(code, joinSessionOptions);

				SystemLogger.Log($"Player {PlayerName} joined. Session; {ActiveSession.Id}");
				Debug.Log($"Player {PlayerName} joined. Session; {ActiveSession.Id}");
			}
			catch (AggregateException ae)
			{
				foreach (var exception in ae.InnerExceptions)
				{
					if (exception is SessionException sessionException)
					{
						if (sessionException.Error == SessionError.SessionNotFound)
						{
							SystemLogger.Log($"Invalid join code");
							Debug.Log($"Invalid join code");
						}
						else if (sessionException.Error == SessionError.Unknown)
						{
							SystemLogger.Log("Unknown. But usually timeout");
							Debug.Log("Unknown. But usually timeout");
						}
						
						_activeSession = null;
						_sessionEventsDispatcher.OnSessionLeft();
						
						SystemLogger.Log($"{exception.Message}");
						Debug.LogException(exception);
					}
					else
					{
						SystemLogger.Log($"Inner Exception: {ae}");
						Debug.Log($"Inner Exception: {ae}");
						Debug.LogException(ae);
					}
				}
			}
			catch (Exception e)
			{
				SystemLogger.Log($"Exception: {e}");
				Debug.Log($"Exception: {e}");
				Debug.LogException(e);
			}
		}

		public async UniTaskVoid KickPlayer(string playerId)
		{
			if (!ActiveSession.IsHost) return;

			await ActiveSession.AsHost().RemovePlayerAsync(playerId);
		}

		public async UniTask LeaveSession()
		{
			if (ActiveSession != null)
			{
				UnRegisterSessionEvents();
				try
				{
					await ActiveSession.LeaveAsync();
				}
				catch (Exception e)
				{
					// ignored
				}
				finally
				{
					ActiveSession = null;
				}
			}
		}
	}
}