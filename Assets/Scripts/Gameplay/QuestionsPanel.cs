using TMPro;
using UnityEngine;

namespace Quiz
{
	public class QuestionsPanel : MonoBehaviour
	{
		[SerializeField] private TMP_Text _amountText;
		[SerializeField] private TMP_Text _questionText;

		private int _totalQuestions;

		public void SetupQuestionPanel(int questionIndex, string question)
		{
			if (GamePlayManager.Instance.IsHost)
			{
				var answers = GamePlayManager.Instance.GetCorrectAnswers(questionIndex);
				Debug.Log($"Correct answers: {string.Join(",", answers)}");
			}
			
			_amountText.SetText($"{questionIndex + 1} / {_totalQuestions}");
			_questionText.SetText($"{question}");
		}

		public void SetTotalQuestions(int totalQuestions)
		{
			_totalQuestions = totalQuestions;
		}
	}
}