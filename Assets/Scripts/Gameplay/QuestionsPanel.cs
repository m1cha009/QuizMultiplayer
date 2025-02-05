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

		public void InitializeQuestions(int totalQuestions)
		{
			_totalQuestions = totalQuestions;
			
			SetTotalQuestions(totalQuestions);
		}

		public void SetTimer(int time)
		{
			_timerText.SetText($"{time:F0} sec");
		}

		public void SetTotalQuestions(int totalQuestions)
		{
			_totalQuestions = totalQuestions;
		}

		public void DisplayQuestion(int questionNumber, string question)
		{
			_amountText.SetText($"{questionNumber} / {_totalQuestions}");
			_questionText.SetText($"{question}");
		}
	}
}