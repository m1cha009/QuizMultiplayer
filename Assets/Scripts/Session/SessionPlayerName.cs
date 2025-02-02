using TMPro;
using UnityEngine;

namespace Quiz
{
	public class SessionPlayerName : MonoBehaviour
	{
		[SerializeField] private TMP_InputField _playerNameInput;

		private void Awake()
		{
			_playerNameInput.onValueChanged.AddListener(OnInputChange);
		}

		private void Start()
		{
			SessionEventsDispatcher.Instance.OnPlayerChangeName(_playerNameInput.text);
		}

		private void OnDestroy()
		{
			_playerNameInput.onValueChanged.RemoveListener(OnInputChange);
		}

		private void OnInputChange(string text)
		{
			SessionManager.Instance.PlayerName = _playerNameInput.text;
			
			SessionEventsDispatcher.Instance.OnPlayerChangeName(_playerNameInput.text);
		}
	}
}