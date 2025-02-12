using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
	public class EndRoundScreen : MonoBehaviour
	{
		[SerializeField] private EndRoundPlayer _endRoundPlayerPrefab;
		[SerializeField] private Transform _rootParent;
		[SerializeField] private Timer _timer;

		private readonly Dictionary<string, EndRoundPlayer> _endRoundPlayers = new();

		private void OnDisable()
		{
			GamePlayManager.Instance.OnTimeChanged -= _timer.SetTimer;
		}

		public void SetupEndRoundScreen(PlayerData[] playersData)
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
			
			GamePlayManager.Instance.OnTimeChanged += _timer.SetTimer;
			
			gameObject.SetActive(true);
		}
	}
}