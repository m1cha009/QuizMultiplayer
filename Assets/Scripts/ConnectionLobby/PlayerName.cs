using TMPro;
using UnityEngine;

namespace Quiz
{
	public class PlayerName : MonoBehaviour
	{
		[SerializeField] private TMP_InputField _playerNameInput;

		private void Awake()
		{
			_playerNameInput.onEndEdit.AddListener(OnEndEdit);
		}

		private void OnEndEdit(string text)
		{
			if (!string.IsNullOrEmpty(_playerNameInput.text))
			{
				SessionManager.Instance.PlayerName = _playerNameInput.text;
			}
		}
	}
}