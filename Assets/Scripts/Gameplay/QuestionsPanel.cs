using System;
using TMPro;
using UnityEngine;

namespace Quiz
{
	public class QuestionsPanel : MonoBehaviour
	{
		[SerializeField] private TMP_Text _amountText;
		[SerializeField] private TMP_Text _questionText;
		[SerializeField] private TMP_Text _timerText;

		private int _totalQuestions;
		private float _timeElapsed;
		private int _questionIndex = 0;
		private GameplayManager _gameplayManager;

		public void OnEnable()
		{
			if (_gameplayManager == null)
			{
				_gameplayManager = GameplayManager.Instance;
			}
			
			_gameplayManager.TimerInitialized = true;
			_totalQuestions = _gameplayManager.TotalQuestionsAmount;

			OnQuestionTimeElapsed();
			
			_gameplayManager.OnQuestionTimeElapsed += OnQuestionTimeElapsed;
		}

		private void OnDisable()
		{
			_gameplayManager.OnQuestionTimeElapsed -= OnQuestionTimeElapsed;
		}

		private void OnQuestionTimeElapsed()
		{
			GameplayManager.Instance.SetupNextQuestionRpc(_questionIndex);
			_questionIndex = (_questionIndex + 1) % _totalQuestions;
		}

		public void SetTimer(int time)
		{
			_timerText.SetText($"{time:F0} sec");
		}

		public void DisplayQuestion(int questionNumber, string question)
		{
			_amountText.SetText($"{questionNumber + 1} / {_totalQuestions}");
			_questionText.SetText($"{question}");
		}
	}
}