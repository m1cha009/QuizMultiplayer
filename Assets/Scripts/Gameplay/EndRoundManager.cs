using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
	public class EndRoundManager : MonoBehaviour
	{
		[SerializeField] private EndRoundPlayer _endRoundPlayerPrefab;
		[SerializeField] private Transform _rootParent;
		[SerializeField] private Timer _timer;

		private readonly int _endRoundTimerDuration = 5;
		
		private readonly Dictionary<string, EndRoundPlayer> _endRoundPlayers = new();

		public void SetupEndRound(List<PlayerData> playersData)
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
			
			_timer.OnTimerEnd += OnTimerEnd;
			_timer.Initialize(_endRoundTimerDuration);
		}

		private void OnTimerEnd()
		{
			_timer.OnTimerEnd -= OnTimerEnd;
			GamePlayManager.Instance.ChangeInnerScreens(InnerScreensType.Gameplay);
		}
	}
}