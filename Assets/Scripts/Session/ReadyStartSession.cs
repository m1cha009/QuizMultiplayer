using System.Collections.Generic;
using System.Linq;
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
			_startButton.gameObject.SetActive(false);
			_readyButton.gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			_startButton.onClick.RemoveListener(OnStartButtonClicked);
			_readyButton.onClick.RemoveListener(OnReadyButtonClicked);
		}

		private void OnStartButtonClicked() // host button
		{
			Debug.Log($"OnStartButtonClicked");
		}

		private void OnReadyButtonClicked() // clients button
		{
			Debug.Log($"OnReadyButtonClicked");
			
			_readyButton.interactable = false;
			
			SessionEventsDispatcher.Instance.OnPlayerReadyRpc(Session.CurrentPlayer.Id, true);
		}

		public void OnSessionJoined()
		{
			if (!_readyPlayerList.TryAdd(Session.CurrentPlayer.Id, false))
			{
				SystemLogger.Log($"Error: Player {Session.CurrentPlayer.Id} is in readyPlayerList");
			}

			if (Session.IsHost)
			{
				_startButton.onClick.AddListener(OnStartButtonClicked);
				_startButton.interactable = false;
				_startButton.gameObject.SetActive(true);
			}
			else
			{
				_readyButton.onClick.AddListener(OnReadyButtonClicked);
				_readyButton.interactable = true;
				_readyButton.gameObject.SetActive(true);
			}
		}

		public void OnSessionLeft()
		{
		}

		public void OnPlayerJoined(string playerId)
		{
			if (!_readyPlayerList.TryAdd(playerId, false))
			{
				SystemLogger.Log($"Error: Player {playerId} is in readyPlayerList");
			}
		}

		public void OnPlayerLeft(string playerId)
		{
			_readyPlayerList.Remove(playerId);
			
			TriggerStartButton();
		}

		public void OnPlayerReadyTrigger(string playerId, bool isReady)
		{
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
	}
}