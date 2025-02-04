using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Quiz
{
	public class PlayerInputPanel : NetworkBehaviour
	{
		[SerializeField] private TMP_InputField _inputField;
		[SerializeField] private PlayerListPanel _playerListPanel;
		
		private void Awake()
		{
			_inputField.onEndEdit.AddListener(OnEndEdit);
			
			TestRpc();
		}

		private void OnEndEdit(string text)
		{
			Debug.Log($"New Text {text}");

			var playerId = GameplayManager.Instance.CurrentPlayerId;
			
			Debug.Log($"Current PlayerId {playerId}");
			
			_playerListPanel.SetPlayerAnswerRpc(playerId, text);
		}

		[Rpc(SendTo.Server)]
		private void TestRpc()
		{
			Debug.Log("sending to server");
		}
	}
}