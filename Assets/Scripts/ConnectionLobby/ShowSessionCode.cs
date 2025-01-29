using Quiz.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quiz
{
	public class ShowSessionCode : BaseSession, ISessionLifecycleEvents
	{
		private TMP_Text _sessionCodeText;
		private Button _copySessionCodeButton;

		private SessionManager _sessionManager;

		private void Awake()
		{
			_sessionCodeText = GetComponentInChildren<TMP_Text>();
			_copySessionCodeButton = GetComponentInChildren<Button>();
		}

		private void Start()
		{
			_sessionManager = SessionManager.Instance;
			
			_copySessionCodeButton.onClick.AddListener(OnCopySessionCodeClick);
		}
		
		private void OnCopySessionCodeClick()
		{
			EventSystem.current.SetSelectedGameObject(null);
			
			if (_sessionManager.ActiveSession == null || string.IsNullOrEmpty(_sessionManager.ActiveSession.Code)) return;

			GUIUtility.systemCopyBuffer = _sessionManager.ActiveSession.Code;
		}

		public void OnSessionJoined()
		{
			if (_sessionManager.ActiveSession == null) return;
			
			_sessionCodeText.SetText(_sessionManager.ActiveSession.Code);
		}
	}
}