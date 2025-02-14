using UnityEngine;

namespace Quiz
{
	public class GameplayScreen : MonoBehaviour
	{
		[SerializeField] private QuestionsPanel _questionsPanel;
		[SerializeField] private Timer _timer;
		[SerializeField] private PlayerListPanel _playerListPanel;
		[SerializeField] private SkillsManager _skillsManager;

		private void OnDisable()
		{
			GamePlayManager.Instance.OnTimeChanged -= _timer.SetTimer;
		}
		
		public void SetupGameplayScreen(int questionIndex, int totalQuestions, string question)
		{
			_playerListPanel.ClearPlayerState();
			_playerListPanel.UpdateTotalPoints();
			_questionsPanel.SetupQuestionPanel(questionIndex, question, totalQuestions);
			_skillsManager.ResetSkills();
			
			GamePlayManager.Instance.OnTimeChanged += _timer.SetTimer;
			
			gameObject.SetActive(true);
		}
	}
}