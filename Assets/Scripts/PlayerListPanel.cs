using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
	public class PlayerListPanel : MonoBehaviour
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
			}
		}

		public void SetPlayerAnswer(string playerId, string answer)
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