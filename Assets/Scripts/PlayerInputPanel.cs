using TMPro;
using UnityEngine;

namespace Quiz
{
	public class PlayerInputPanel : MonoBehaviour
	{
		[SerializeField] private TMP_InputField _inputField;
		[SerializeField] private PlayerListPanel _playerListPanel;
		
		private void Awake()
		{
			_inputField.onEndEdit.AddListener(OnEndEdit);
		}

		private void OnEndEdit(string text)
		{
			var playerId = GameplayManager.Instance.CurrentPlayerId;
			
			GameplayManager.Instance.SetAnswerRpc(playerId, text);
		}
	}
}