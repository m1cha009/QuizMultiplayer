using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Quiz
{
	public class PlayerListPanel : NetworkBehaviour, IGameplayBaseEvents, IGameplayLifecycleEvents, ISessionEvents
	{
		[SerializeField] private Player _playerPrefab;

		private Dictionary<string, PlayerData> _playersDataDic;
		private readonly Dictionary<string, Player> _playersDic = new();

		private void Start()
		{
			GameplayEventDispatcher.Instance.RegisterGameplayEvents(this);
		}
		
		public void OnGameplayStarted()
		{
			_playersDataDic = GameManager.Instance.GetPlayersData();

			if (_playersDataDic == null)
			{
				Debug.Log("Players data is null");
				
				return;
			}
			
			InitializePlayers(_playersDataDic);
		}

		public void OnGameplayStopped() { }
		
		public void OnPlayerJoined(string playerId)
		{
			InitializePlayer(playerId);
		}

		public void OnPlayerLeft(string playerId)
		{
			DeInitializePlayer(playerId);
		}

		public void OnPlayerReadyTrigger(string playerId, bool isReady) { }

		public void OnSessionDeleted()
		{
			DeInitializePlayers();
		}
		
		[Rpc(SendTo.ClientsAndHost)]
		public void SetPlayerAnswerRpc(string playerId, string answer)
		{
			if (_playersDic.Count == 0 || !_playersDic.ContainsKey(playerId))
			{
				Debug.Log($"Player {playerId} not found");
				
				return;
			}

			_playersDic.TryGetValue(playerId, out var player);
			if (player != null) player.SetAnswer(answer);

			if (_playersDataDic.TryGetValue(playerId, out var playerData))
			{
				playerData.Answer = answer;
			}
		}

		public void CleanAnswers()
		{
			foreach (var player in _playersDic.Values)
			{
				player.SetAnswer(string.Empty);
			}
		}

		private void InitializePlayers(Dictionary<string, PlayerData> playersData)
		{
			foreach (var playerData in playersData)
			{
				InitializePlayer(playerData.Key);
			}
		}

		private void InitializePlayer(string playerId)
		{
			if (_playersDic.ContainsKey(playerId)) return;
			if (!_playersDataDic.TryGetValue(playerId, out var playerData)) return;
			
			var player = Instantiate(_playerPrefab, transform);
			player.SetName(playerData.PlayerName);
			player.SetAnswer(playerData.Answer);
			
			_playersDic.Add(playerData.PlayerId, player);
		}

		private void DeInitializePlayer(string playerId)
		{
			if (_playersDic.TryGetValue(playerId, out var player))
			{
				Debug.Log($"Player {playerId} deInitialized from PlayerListPanel");
				
				Destroy(player.gameObject);
				_playersDic.Remove(playerId);
			}

			_playersDataDic.Remove(playerId, out var playerData);
		}

		private void DeInitializePlayers()
		{
			foreach (var player in _playersDic)
			{
				Destroy(player.Value.gameObject);
			}
			
			_playersDic.Clear();
			_playersDataDic.Clear();
		}
	}
}