using TMPro;
using UnityEngine;

namespace Quiz
{
	public class PlayerInputPanel : MonoBehaviour, IGameplayBaseEvents, IGameplayLifecycleEvents
	{
		[SerializeField] private TMP_InputField _inputField;
		[SerializeField] private PlayerListPanel _playerListPanel;
		
		private void Start()
		{
			GameplayEventDispatcher.Instance.RegisterGameplayEvents(this);
		}
		
		public void OnGameplayStarted()
		{
			_inputField.onEndEdit.AddListener(OnEndEdit);
		}

		public void OnGameplayStopped()
		{
			_inputField.onEndEdit.RemoveListener(OnEndEdit);
		}

		private void OnEndEdit(string text)
		{
			var playerId = GameManager.Instance.CurrentPlayerId;
			
			_playerListPanel.SetPlayerAnswerRpc(playerId, text);
			
			GamePlayManager.Instance.AddOrderedAnswerRpc(playerId, text);
			
			_inputField.SetTextWithoutNotify(string.Empty);
		}
	}
}