using UnityEngine;

namespace Quiz
{
	public class GameplayScreen : MonoBehaviour
	{
		[SerializeField] private QuestionsPanel _questionsPanel;
		[SerializeField] private Timer _timer;
		[SerializeField] private PlayerListPanel _playerListPanel;

		private void OnDisable()
		{
			GamePlayManager.Instance.OnTimeChanged -= _timer.SetTimer;
		}
		
		public void SetupGameplayScreen(int questionIndex, int totalQuestions, string question)
		{
			_questionsPanel.SetTotalQuestions(totalQuestions);
			_questionsPanel.SetupQuestionPanel(questionIndex, question);
			
			GamePlayManager.Instance.OnTimeChanged += _timer.SetTimer;
			
			gameObject.SetActive(true);
		}

		public void ClearAnswers()
		{
			_playerListPanel.CleanAnswers();
		}
	}
}