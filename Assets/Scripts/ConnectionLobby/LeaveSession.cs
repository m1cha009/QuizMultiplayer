using UnityEngine;
using UnityEngine.UI;

namespace Quiz
{
	public class LeaveSession : MonoBehaviour
	{
		private Button _leaveSessionButton;

		private void Awake()
		{
			_leaveSessionButton = GetComponentInChildren<Button>();
			_leaveSessionButton.onClick.AddListener(OnLeaveSessionClicked);
		}

		private void OnLeaveSessionClicked()
		{
			SessionManager.Instance.LeaveSession();
		}
	}
}