using TMPro;
using UnityEngine.UI;

namespace Quiz
{
	public class JoinSessionByCode : BaseSession, ISessionLifecycleEvents, IPlayerNameEvents
	{
		private TMP_InputField _inputField;
		private Button _joinButton;
		
		protected void Awake()
		{
			_inputField = GetComponentInChildren<TMP_InputField>();
			_joinButton = GetComponentInChildren<Button>();
			_joinButton.onClick.AddListener(OnJoinButtonClicked);
		}

		private void OnDisable()
		{
			_joinButton.onClick.RemoveListener(OnJoinButtonClicked);
		}

		private async void OnJoinButtonClicked()
		{
			if (!string.IsNullOrEmpty(_inputField.text))
			{
				_joinButton.interactable = false;
			
				await SessionManager.Instance.JoinSessionByJoinCode(_inputField.text);
			}
		}

		public void OnSessionJoined()
		{
			_joinButton.interactable = false;
		}

		public void OnSessionLeft()
		{
			_joinButton.interactable = true;
		}

		public void OnPlayerNameChange(string playerName)
		{
			_joinButton.interactable = !string.IsNullOrEmpty(playerName);
			_inputField.interactable = !string.IsNullOrEmpty(playerName);
		}
	}
}