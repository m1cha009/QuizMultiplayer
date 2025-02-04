using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
	public class PlayerListPanel : MonoBehaviour
	{
		[SerializeField] private Player _playerPrefab;

		private GameplayManager _gameplayManager;
		private Dictionary<string, Player> _playerList = new();

		private void Start()
		{
			_gameplayManager = GameplayManager.Instance;
		}

		public void InitializePlayers()
		{
			var playersDataList = _gameplayManager.GetPlayersData();

			if (playersDataList == null)
			{
				return;
			}

			foreach (var playerData in playersDataList)
			{
				if (_playerList.ContainsKey(playerData.PlayerId))
				{
					continue;
				}
				
				var player = Instantiate(_playerPrefab, transform);
				player.SetName(playerData.PlayerName);
				player.SetAnswer(playerData.Answer);
				
				_playerList.Add(playerData.PlayerId, player);
			}
		}
	}
}