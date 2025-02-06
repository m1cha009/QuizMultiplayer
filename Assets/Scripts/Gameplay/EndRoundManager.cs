using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
	public class EndRoundManager : MonoBehaviour
	{
		[SerializeField] private EndRoundPlayer _endRoundPlayerPrefab;
		[SerializeField] private Transform _rootParent;
		
		private readonly Dictionary<string, EndRoundPlayer> _endRoundPlayers = new();

		public void InitializePlayers(List<PlayerData> playersData)
		{
			foreach (var playerData in playersData)
			{
				if (!_endRoundPlayers.ContainsKey(playerData.PlayerId))
				{
					var player = Instantiate(_endRoundPlayerPrefab, _rootParent);
					_endRoundPlayers.Add(playerData.PlayerId, player);
				}
				
				_endRoundPlayers[playerData.PlayerId].Setup(playerData);
			}
		}
	}
}