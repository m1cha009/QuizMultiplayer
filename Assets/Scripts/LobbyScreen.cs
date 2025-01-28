using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
	public class LobbyScreen : MonoBehaviour
	{
		[SerializeField] private TMP_Text _playersNameText;

		public void SetPlayerName(string name)
		{
			_playersNameText.text += $"{name}\n";
		}
		
		public void ClearPlayerNames() => _playersNameText.text = string.Empty;

		public void Show()
		{
			gameObject.SetActive(true);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}