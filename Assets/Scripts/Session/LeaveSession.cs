using System;
using UnityEngine;
using UnityEngine.UI;

namespace Quiz
{
	public class LeaveSession : BaseSession, ISessionLifecycleEvents
	{
		private Button _leaveSessionButton;

		private void Awake()
		{
			_leaveSessionButton = GetComponentInChildren<Button>();
			_leaveSessionButton.onClick.AddListener(OnLeaveSessionClicked);

			DefaultState();
		}

		private void OnDestroy()
		{
			_leaveSessionButton.onClick.RemoveListener(OnLeaveSessionClicked);
		}

		private async void OnLeaveSessionClicked()
		{
			_leaveSessionButton.interactable = false;
			
			await SessionManager.Instance.LeaveSession();
			
			SystemLogger.Log("Left server");
			Debug.Log("Left server");
		}

		public void OnSessionJoined()
		{
			_leaveSessionButton.interactable = true;
		}

		public void OnSessionLeft()
		{
			DefaultState();
		}

		private void DefaultState()
		{
			_leaveSessionButton.interactable = false;
		}
	}
}