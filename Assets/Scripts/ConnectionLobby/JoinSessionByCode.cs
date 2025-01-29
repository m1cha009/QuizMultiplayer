using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quiz
{
	public class JoinSessionByCode : MonoBehaviour
	{
		private TMP_InputField _inputField;
		private Button _joinButton;
		
		protected void OnEnable()
		{
			_inputField = GetComponentInChildren<TMP_InputField>();
			_joinButton = GetComponentInChildren<Button>();
			_joinButton.onClick.AddListener(OnJoinButtonClicked);
		}

		private void OnDisable()
		{
			_joinButton.onClick.RemoveListener(OnJoinButtonClicked);
		}

		private void OnJoinButtonClicked()
		{
			if (!string.IsNullOrEmpty(_inputField.text))
			{
				SessionManager.Instance.JoinSessionByJoinCode(_inputField.text);
			}
		}
	}
}