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
		}

		private void UnRegisterSessionEvents()
		{
			ActiveSession.PlayerJoined -= _sessionEventsDispatcher.OnPlayerJoined;
		}

		private async UniTask<Dictionary<string, PlayerProperty>> GetPlayerProperties()
		{
			var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
			var playerNameProperties = new PlayerProperty(playerName, VisibilityPropertyOptions.Member);

			return new Dictionary<string, PlayerProperty>
			{
				{ playerNameProperty, playerNameProperties }
			};
		}

		public async UniTask StartSessionAsHost()
		{
			var playerProperties = await GetPlayerProperties();
			
			var options = new SessionOptions()
			{
				MaxPlayers = 6,
				PlayerProperties = playerProperties
			}.WithDistributedAuthorityNetwork();


			ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
			
			RegisterSessionEvents();
			
			Debug.Log($"Session {ActiveSession.Id} created! Join code: {ActiveSession.Code}");
		}

		public async UniTaskVoid JoinSessionByJoinCode(string code)
		{
			try
			{
				ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(code);

				RegisterSessionEvents();
				Debug.Log($"Session {ActiveSession.Id} joined");
			}
			catch (AggregateException ae)
			{
				foreach (var exception in ae.InnerExceptions)
				{
					if (exception is SessionException sessionException)
					{
						if (sessionException.Error == SessionError.SessionNotFound)
						{
							Debug.Log($"Invalid join code");
						}
					}
					else
					{
						Debug.LogException(ae);
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public async UniTaskVoid KickPlayer(string playerId)
		{
			if (!ActiveSession.IsHost) return;

			await ActiveSession.AsHost().RemovePlayerAsync(playerId);
		}

		public async UniTaskVoid LeaveSession()
		{
			if (ActiveSession != null)
			{
				try
				{
					UnRegisterSessionEvents();
					await ActiveSession.LeaveAsync();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
				finally
				{
					ActiveSession = null;
				}
			}
		}
	}
}