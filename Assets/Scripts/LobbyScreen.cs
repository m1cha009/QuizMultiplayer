using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
	public class LobbyScreen : MonoBehaviour
	{
		[SerializeField] private TMP_Text _playersNameText;
		[SerializeField] private TMP_Text _systemText;

		public void SetPlayerName(string name)
		{
			_playersNameText.text += $"{name}\n";
		}
		
		public void ClearPlayerNames() => _playersNameText.text = string.Empty;

		public void Show()
		{
			gameObject.SetActive(true);
		}

		private void Hide()
		{
			gameObject.SetActive(false);
		}

		public void Disconnect()
		{
			ScreensManager.Instance.DisconnectPlayerRpc();
			ScreensManager.Instance.ChangeToScreen(ScreensManager.ScreensEnum.ConnectionScreen);
			
			Hide();
		}
	}
}