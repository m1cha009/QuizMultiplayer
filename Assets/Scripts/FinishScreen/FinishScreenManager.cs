using System;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace Quiz
{
	public class FinishScreenManager : MonoBehaviour, ISessionProvider
	{
		[SerializeField] private Button _backToLobby;
		[SerializeField] private Button _exitButton;
		
		public ISession Session { get; set; }

		private void Awake()
		{
			_backToLobby.onClick.AddListener(OnBackToLobbyClicked);
			_exitButton.onClick.AddListener(OnExitClicked);
		}

		private void OnDestroy()
		{
			_backToLobby.onClick.RemoveListener(OnBackToLobbyClicked);
			_exitButton.onClick.RemoveListener(OnExitClicked);
		}

		private void OnBackToLobbyClicked()
		{
			GameManager.Instance.ChangeScreenRpc(ScreensType.Lobby);
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

		
	}
}