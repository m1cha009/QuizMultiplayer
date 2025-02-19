using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Quiz
{
	public class PlayerListPanel : NetworkBehaviour, IGameplayBaseEvents, IGameplayLifecycleEvents, ISessionEvents
	{
		[SerializeField] private PlayerView _playerViewPrefab;
		[SerializeField] private SkillsManager _skillsManager;

		private Dictionary<string, Player> _playersDic;
		private readonly Dictionary<string, PlayerView> _playersViewDic = new();

		private string _localPlayerId;

		private void Start()
		{
			GameplayEventDispatcher.Instance.RegisterGameplayEvents(this);
		}
		
		public void OnGameplayStarted()
		{
			_playersDic = GameManager.Instance.GetPlayersDictionary();

			if (_playersDic == null)
			{
				Debug.Log("Players data is null");
				
				return;
			}

			_localPlayerId = GameManager.Instance.CurrentPlayerId;
			
			InitializePlayers(_playersDic);
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
			if (_playersViewDic.Count == 0 || !_playersViewDic.ContainsKey(playerId))
			{
				Debug.Log($"Player {playerId} not found");
				
				return;
			}

			_playersViewDic.TryGetValue(playerId, out var playerView);
			if (playerView != null) playerView.SetAnswer(answer);

			if (_playersDic.TryGetValue(playerId, out var player))
			{
				var playerData = player.PlayerData;
				playerData.Answer = answer;
			}
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void SetPlayerSkillIconRpc(string playerId, SkillType skillType)
		{
			_playersViewDic.TryGetValue(playerId, out var playerView);
			if (playerView != null) playerView.SetSkillType(true, skillType);
		}
		
		[Rpc(SendTo.Server)]
		private void SetServerSkillDataRpc(string ownerId, SkillType skillType, string targetId) // new
		{
			_playersDic.TryGetValue(targetId, out var targetPlayer);
			_playersDic.TryGetValue(ownerId, out var ownerPlayer);
			if (targetPlayer == null || ownerPlayer == null) return;

			var playerSkillData = new PlayerSkillData
			{
				SkillType = skillType,
				AttackerId = ownerId,
				AttackerName = ownerPlayer.PlayerData.PlayerName,
				SkillPrice = _skillsManager.GetSkillPrice(skillType)
			};
			
			targetPlayer.AddPLayerSkillData(playerSkillData);
		}

		public void ClearPlayerState()
		{
			foreach (var player in _playersViewDic.Values)
			{
				player.SetAnswer(string.Empty);
				player.SetSkillTargetColor(true);
				player.SetSkillType(false, SkillType.None);
			}
		}

		public void UpdateTotalPoints()
		{
			foreach (var keyValuePair in _playersViewDic)
			{
				var playerId = keyValuePair.Key;
				var playerView = keyValuePair.Value;

				if (_playersDic.TryGetValue(playerId, out var player))
				{
					playerView.SetTotalPoints(player.PlayerData.TotalPoints);
				}
			}
		}

		private void InitializePlayers(Dictionary<string, Player> playersData)
		{
			foreach (var playerData in playersData)
			{
				InitializePlayer(playerData.Key);
			}
		}

		private void InitializePlayer(string playerId)
		{
			if (_playersViewDic.ContainsKey(playerId)) return;
			if (!_playersDic.TryGetValue(playerId, out var pl)) return;

			var playerData = pl.PlayerData;
			
			var player = Instantiate(_playerViewPrefab, transform);
			player.SetPlayerId(playerId);
			player.SetName(_localPlayerId == playerId ? $"{playerData.PlayerName} (YOU)" : playerData.PlayerName);
			player.SetAnswer(playerData.Answer);
			player.SetTotalPoints(playerData.TotalPoints);
			player.SetSkillTargetColor(true);
			player.PlayerClickEvent += OnPlayerClicked;
			
			_playersViewDic.Add(playerData.PlayerId, player);
		}

		private void OnPlayerClicked(string targetId, PlayerView clickedPlayerView)
		{
			if (_skillsManager.SelectedSkillType != SkillType.None && !_skillsManager.IsSkillUsed)
			{
				_skillsManager.IsSkillUsed = true;
				
				clickedPlayerView.SetSkillTargetColor(false); // for local player set selected player color

				SetPlayerSkillIconRpc(_localPlayerId, _skillsManager.SelectedSkillType); // for all display that user has used skill
				
				SetServerSkillDataRpc(_localPlayerId, _skillsManager.SelectedSkillType, targetId);
			}
		}

		private void DeInitializePlayer(string playerId)
		{
			if (_playersViewDic.TryGetValue(playerId, out var player))
			{
				Debug.Log($"Player {playerId} deInitialized from PlayerListPanel");
				
				player.PlayerClickEvent -= OnPlayerClicked;
				
				Destroy(player.gameObject);
				_playersViewDic.Remove(playerId);
			}

			_playersDic.Remove(playerId, out var playerData);
		}

		private void DeInitializePlayers()
		{
			foreach (var player in _playersViewDic)
			{
				Destroy(player.Value.gameObject);
			}
			
			_playersViewDic.Clear();
			_playersDic.Clear();
		}
	}
}