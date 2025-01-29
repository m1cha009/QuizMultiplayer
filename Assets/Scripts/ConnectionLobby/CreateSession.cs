using Quiz.Interfaces;
using UnityEngine.UI;

namespace Quiz
{
	public class CreateSession : BaseSession, IPlayerNameEvents, ISessionLifecycleEvents
	{
		private Button _createSessionButton;

		private void Awake()
		{
			_createSessionButton = GetComponentInChildren<Button>();
			_createSessionButton.onClick.AddListener(OnCreateSessionClicked);
		}

		private void OnDestroy()
		{
			_createSessionButton.onClick.RemoveListener(OnCreateSessionClicked);
		}

		private async void OnCreateSessionClicked()
		{
			_createSessionButton.interactable = false;
			
			await SessionManager.Instance.StartSessionAsHost();
		}

		public void OnPlayerNameChange(string playerName)
		{
			_createSessionButton.interactable = !string.IsNullOrEmpty(playerName);
		}

		public void OnSessionJoined()
		{
			_createSessionButton.interactable = false;
		}

		public void OnSessionLeft()
		{
			_createSessionButton.interactable = true;
		}
	}
}