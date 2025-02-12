using TMPro;
using UnityEngine;

namespace Quiz
{
	public class QuestionsPanel : MonoBehaviour
	{
		[SerializeField] private TMP_Text _amountText;
		[SerializeField] private TMP_Text _questionText;

		private int _totalQuestions;

		public void SetupQuestionPanel(int questionIndex)
		{
			SetQuestion(questionIndex);
		}

		public void SetTotalQuestions(int totalQuestions)
		{
			_totalQuestions = totalQuestions;
		}

		private void SetQuestion(int questionIndex)
		{
			if (GamePlayManager.Instance.IsHost)
			{
				var answers = GamePlayManager.Instance.GetCorrectAnswers(questionIndex);
				Debug.Log($"Correct answers: {string.Join(",", answers)}");
			}
			
			_amountText.SetText($"{questionIndex + 1} / {_totalQuestions}");

			var question = GamePlayManager.Instance.CurrentQuestion;
			_questionText.SetText($"{question.Value}");
		}
	}
}