using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quiz
{
	public class SessionPlayerItem : MonoBehaviour
	{
		[SerializeField] private TMP_Text _playerName;
		[SerializeField] private Button _kickButton;
		[SerializeField] private TMP_Text _readyText;

		private string _playerId;

		public string PlayerName { get; private set; }

		public void Init(string playerId, string playerName)
		{
			_playerId = playerId;
			_playerName.SetText(playerName);
			_readyText.SetText("Not Ready");
			PlayerName = playerName;
			
			_kickButton.onClick.AddListener(OnKickButtonClicked);
		}

		public void SetReady(bool isReady)
		{
			_readyText.SetText(isReady ? "Ready" : "Not Ready");
		}

		private void OnDestroy()
		{
			_playerId = null;
			
			_kickButton.onClick.RemoveListener(OnKickButtonClicked);
		}

		private void OnKickButtonClicked()
		{
			_ = SessionManager.Instance.KickPlayer(_playerId);
		}
	}
}