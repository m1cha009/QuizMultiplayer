using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quiz
{
	public class SessionPlayerItem : MonoBehaviour
	{
		[SerializeField] private TMP_Text _playerName;
		[SerializeField] private Button _kickButton;

		private string _playerId;

		public void Init(string playerId, string playerName)
		{
			_playerId = playerId;
			_playerName.SetText(playerName);
			
			_kickButton.onClick.AddListener(OnKickButtonClicked);
			
			Debug.Log("Session player item initialized");
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