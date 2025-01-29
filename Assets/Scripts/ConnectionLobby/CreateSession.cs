using UnityEngine;
using UnityEngine.UI;

namespace Quiz
{
	public class CreateSession : MonoBehaviour
	{
		private Button _createSessionButton;

		private void Awake()
		{
			_createSessionButton = GetComponentInChildren<Button>();
			_createSessionButton.onClick.AddListener(OnCreateSessionClicked);
		}

		private void OnCreateSessionClicked()
		{
			SessionManager.Instance.StartSessionAsHost();
		}
	}
}