using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace Quiz
{
	public class LeaveSession : BaseSession, ISessionLifecycleEvents
	{
		public ISession Session { get; set; }
		
		private Button _leaveSessionButton;

		private void Awake()
		{
			_leaveSessionButton = GetComponentInChildren<Button>();
			_leaveSessionButton.onClick.AddListener(OnLeaveSessionClicked);

			_leaveSessionButton.interactable = false;
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
			_leaveSessionButton.interactable = false;
		}
	}
}