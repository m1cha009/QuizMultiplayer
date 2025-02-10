using System;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace Quiz
{
	public class FinishScreenManager : BaseScreens, ISessionProvider, IBaseSession, ISessionLifecycleEvents
	{
		[SerializeField] private Button _restartGame;
		[SerializeField] private Button _backToLobby;
		[SerializeField] private Button _exitButton;
		
		public ISession Session { get; set; }

		private void Awake()
		{
			_restartGame.onClick.AddListener(OnRestartClicked);
			_backToLobby.onClick.AddListener(OnBackToLobbyClicked);
			_exitButton.onClick.AddListener(OnExitClicked);
		}

		private void OnEnable()
		{
			SessionEventsDispatcher.Instance.RegisterBaseClassEvents(this);
		}

		private void OnDestroy()
		{
			_restartGame.onClick.RemoveListener(OnRestartClicked);
			_backToLobby.onClick.RemoveListener(OnBackToLobbyClicked);
			_exitButton.onClick.RemoveListener(OnExitClicked);
		}

		private void OnRestartClicked()
		{
			if (Session.IsHost)
			{
				GameManager.Instance.ChangeScreenRpc(ScreensType.GamePlay);
			}
		}

		private void OnBackToLobbyClicked()
		{
			GameManager.Instance.ChangeScreen(ScreensType.Lobby);
		}
		
		private async void OnExitClicked()
		{
			// Quit the session
			try
			{
				await Session.LeaveAsync();
			}
			catch (Exception)
			{
				// ignore
			}
			finally
			{
				Session = null;
			}
			
			
			Application.Quit();
		}


		public void OnSessionJoined()
		{
			if (!Session.IsHost)
			{
				_restartGame.interactable = false;
			}
		}

		public void OnSessionLeft()
		{
		}
	}
}