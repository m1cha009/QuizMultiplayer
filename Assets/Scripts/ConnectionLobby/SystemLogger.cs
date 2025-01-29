using TMPro;
using UnityEngine;

namespace Quiz
{
	public class SystemLogger : MonoBehaviour
	{
		private static SystemLogger _instance;
		
		private static SystemLogger Instance => _instance;

		[SerializeField] private Transform _parent;
		[SerializeField] private TMP_Text _tmpText;

		private void Awake()
		{
			if (_instance != null && _instance != this)
			{
				Destroy(gameObject);
			}
			else
			{
				_instance = this;
			}
		}

		public static void Log(string message)
		{
			Instance.DisplayMessage(message);
		}

		private void DisplayMessage(string message)
		{
			var tmpTextInstance = Instantiate(_tmpText, _parent);
			tmpTextInstance.SetText($"[{System.DateTime.Now:HH:mm:ss}] {message}");
		}
	}
}