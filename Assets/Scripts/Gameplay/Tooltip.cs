using TMPro;
using UnityEngine;

namespace Quiz
{
	public class Tooltip : MonoBehaviour
	{
		[SerializeField] private TMP_Text _messageText;

		public Vector2 TooltipOffset => new Vector2(10f, 15f);

		public void SetMessage(string message) => _messageText.SetText(message);
		
		public void Show() => gameObject.SetActive(true);
		public void Hide() => gameObject.SetActive(false);
	}
}