using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Quiz
{
	public class PlayerListPanel : NetworkBehaviour
	{
		[SerializeField] private Player _playerPrefab;

		private readonly Dictionary<string, Player> _playerDic = new();

		public void InitializePlayers()
		{
			var playersDataList = GameplayManager.Instance.GetPlayersData();

			if (playersDataList == null)
			{
				return;
			}

			foreach (var playerData in playersDataList)
			{
				if (_playerDic.ContainsKey(playerData.PlayerId))
				{
					continue;
				}
				
				var player = Instantiate(_playerPrefab, transform);
				player.SetName(playerData.PlayerName);
				player.SetAnswer(playerData.Answer);
				
				_playerDic.Add(playerData.PlayerId, player);
				
				Debug.Log($"Instantiated playerID: {playerData.PlayerId}");
			}
		}

		[Rpc(SendTo.Server)]
		public void SetPlayerAnswerRpc(string playerId, string answer)
		{
			if (_playerDic.Count == 0 || !_playerDic.ContainsKey(playerId))
			{
				Debug.Log($"Player {playerId} not found");
				
				return;
			}

			_playerDic.TryGetValue(playerId, out var player);
			if (player != null) player.SetAnswer(answer);
		}
	}
}