using UnityEngine;

namespace Quiz
{
	public class GameplayScreen : MonoBehaviour, IGameplayBaseEvents, IGameplayLifecycleEvents
	{
		[SerializeField] private QuestionsPanel _questionsPanel;
		[SerializeField] private Timer _timer;
		[SerializeField] private PlayerListPanel _playerListPanel;

		private void Start()
		{
			GameplayEventDispatcher.Instance.RegisterGameplayEvents(this);
		}

		private void OnDisable()
		{
			GamePlayManager.Instance.OnTimeChanged -= _timer.SetTimer;
		}

		public void OnGameplayStarted()
		{
			_questionsPanel.SetTotalQuestions(GamePlayManager.Instance.TotalQuestionsAmount);
			SetupGameplayScreen(0);
		}

		public void OnGameplayStopped()
		{
			GamePlayManager.Instance.OnTimeChanged -= _timer.SetTimer;
		}
		
		public void SetupGameplayScreen(int questionIndex)
		{
			_questionsPanel.SetupQuestionPanel(questionIndex);
			
			GamePlayManager.Instance.OnTimeChanged += _timer.SetTimer;
		}

		public void ClearAnswers()
		{
			_playerListPanel.CleanAnswers();
		}
	}
}