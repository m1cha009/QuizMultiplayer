using TMPro;
using UnityEngine;

namespace Quiz
{
	public class QuestionsPanel : MonoBehaviour
	{
		[SerializeField] private TMP_Text _amountText;
		[SerializeField] private TMP_Text _questionText;

		public void SetupQuestionPanel(int questionIndex, string question, int totalQuestions)
		{
			if (GamePlayManager.Instance.IsHost)
			{
				var answers = GamePlayManager.Instance.GetCorrectAnswers(questionIndex);
				Debug.Log($"Correct answers: {string.Join(",", answers)}");
			}
			
			_amountText.SetText($"{questionIndex + 1} / {totalQuestions}");
			_questionText.SetText($"{question}");
		}
	}
}