using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Quiz
{
	public class PlayerListPanel : NetworkBehaviour, IGameplayBaseEvents, IGameplayLifecycleEvents, ISessionEvents
	{
		[SerializeField] private Player _playerPrefab;
		[SerializeField] private SkillsManager _skillsManager;

		private Dictionary<string, PlayerData> _playersDataDic;
		private readonly Dictionary<string, Player> _playersDic = new();

		private string _localPlayerId;

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

			_localPlayerId = GameManager.Instance.CurrentPlayerId;
			
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

		[Rpc(SendTo.ClientsAndHost)]
		private void SetPlayerSkillIconRpc(string playerId, SkillType skillType)
		{
			_playersDic.TryGetValue(playerId, out var player);
			if (player != null) player.SetSkillType(true, skillType);
		}

		public void ClearPlayerState()
		{
			foreach (var player in _playersDic.Values)
			{
				player.SetAnswer(string.Empty);
				player.SetSkillTargetColor(true);
				player.SetSkillType(false, SkillType.None);
			}
		}

		public void UpdateTotalPoints()
		{
			foreach (var keyValuePair in _playersDic)
			{
				var playerId = keyValuePair.Key;
				var player = keyValuePair.Value;

				if (_playersDataDic.TryGetValue(playerId, out var playerData))
				{
					player.SetTotalPoints(playerData.TotalPoints);
				}
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
			player.SetName(_localPlayerId == playerId ? $"{playerData.PlayerName} (YOU)" : playerData.PlayerName);
			player.SetAnswer(playerData.Answer);
			player.SetTotalPoints(playerData.TotalPoints);
			player.SetSkillTargetColor(true);
			player.PlayerClickEvent += OnPlayerClicked;
			
			_playersDic.Add(playerData.PlayerId, player);
		}

		private void OnPlayerClicked(Player playerClicked)
		{
			if (_skillsManager.SelectedSkillType != SkillType.None)
			{
				playerClicked.SetSkillTargetColor(false);

				SetPlayerSkillIconRpc(_localPlayerId, _skillsManager.SelectedSkillType);
			}
		}

		private void DeInitializePlayer(string playerId)
		{
			if (_playersDic.TryGetValue(playerId, out var player))
			{
				Debug.Log($"Player {playerId} deInitialized from PlayerListPanel");
				
				player.PlayerClickEvent -= OnPlayerClicked;
				
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