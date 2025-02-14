using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace Quiz
{
	public class ReadyStartSession : BaseSession, ISessionProvider, ISessionLifecycleEvents, ISessionEvents
	{
		public ISession Session { get; set; }
		
		[SerializeField] private Button _startButton;
		[SerializeField] private Button _readyButton;
		
		private readonly Dictionary<string, bool> _readyPlayerList = new();

		private void Awake()
		{
			DefaultState();

			_startButton.interactable = true;
		}

		private void OnDestroy()
		{
			DefaultState();
		}

		private void OnStartButtonClicked() // host button
		{
			GameManager.Instance.ChangeScreenRpc(ScreensType.GamePlay);
		}

		private async void OnReadyButtonClicked() // clients button
		{
			_readyButton.interactable = false;

			await Task.Delay(500);
			
			SessionEventsDispatcher.Instance.OnPlayerReadyRpc(Session.CurrentPlayer.Id, true);
		}

		public void OnSessionJoined()
		{
			if (Session.IsHost)
			{
				_readyPlayerList.Clear();
				
				if (!_readyPlayerList.TryAdd(Session.CurrentPlayer.Id, false))
				{
					SystemLogger.Log($"Error: Player {Session.CurrentPlayer.Id} is in readyPlayerList");
				}
				
				_startButton.gameObject.SetActive(true);
				_startButton.onClick.AddListener(OnStartButtonClicked);
			}
			else
			{
				_readyButton.interactable = true;
				_readyButton.gameObject.SetActive(true);
				_readyButton.onClick.AddListener(OnReadyButtonClicked);
			}
		}

		public void OnSessionLeft()
		{
			DefaultState();
		}
		
		public void OnSessionDeleted()
		{
			if (Session.IsHost)
			{
				_readyPlayerList.Remove(Session.CurrentPlayer.Id);
			}

			DefaultState();
		}

		public void OnPlayerJoined(string playerId)
		{
			if (!Session.IsHost) return;
			
			if (!_readyPlayerList.TryAdd(playerId, false))
			{
				SystemLogger.Log($"Error: Player {playerId} is in readyPlayerList");
			}
		}

		public void OnPlayerLeft(string playerId)
		{
			if (!Session.IsHost) return;
			
			_readyPlayerList.Remove(playerId);
			
			TriggerStartButton();
		}

		public void OnPlayerReadyTrigger(string playerId, bool isReady)
		{
			if (!Session.IsHost) return;

			_readyPlayerList[playerId] = isReady;

			TriggerStartButton();
		}

		private void TriggerStartButton()
		{
			if (!Session.IsHost) return;
			
			var allReady = _readyPlayerList
				.Where(x => x.Key != Session.CurrentPlayer.Id)
				.All( ready => ready.Value == true) && _readyPlayerList.Count > 1;

			_startButton.interactable = allReady;
		}

		private void DefaultState()
		{
			_startButton.onClick.RemoveListener(OnStartButtonClicked);
			_readyButton.onClick.RemoveListener(OnReadyButtonClicked);
			_startButton.interactable = false;
			_readyButton.interactable = false;
			_startButton.gameObject.SetActive(false);
			_readyButton.gameObject.SetActive(false);
		}
	}
}