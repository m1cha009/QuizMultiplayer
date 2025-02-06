using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Quiz
{
	public class Timer : NetworkBehaviour
	{
		[SerializeField] private TMP_Text _timerText;
		
		public event Action OnTimerEnd;

		private readonly NetworkVariable<int> _serverTimeLeft = new();
		private readonly float _syncInterval = 1f;
		private float _lastSyncTime;
		private float _localTimeLeft;
		
		private bool _isSet;

		private void Update()
		{
			if (!_isSet || !IsServer) return;
			
			if (_localTimeLeft > 0)
			{
				_localTimeLeft -= Time.deltaTime;
			
				if (Time.time - _lastSyncTime >= _syncInterval)
				{
					_lastSyncTime = Time.time;
					_serverTimeLeft.Value = Mathf.FloorToInt(_localTimeLeft); // it is going to call OnTimerValueChanged
				}
			}
			else
			{
				OnTimerEndRpc();
				_serverTimeLeft.OnValueChanged -= OnTimerValueChanged;
				_isSet = false;
			}
		}
		
		public void Initialize(int timerDuration)
		{
			_localTimeLeft = timerDuration;
			_lastSyncTime = 0;
			_serverTimeLeft.OnValueChanged += OnTimerValueChanged;
			_isSet = true;
		}
		
		private void OnTimerValueChanged(int previousValue, int newValue)
		{
			_timerText.SetText($"{newValue:F0} sec");
		}

		[Rpc(SendTo.ClientsAndHost)]
		private void OnTimerEndRpc()
		{
			OnTimerEnd?.Invoke();
		}
	}
}