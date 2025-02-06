using TMPro;
using UnityEngine;

namespace Quiz
{
	public class QuestionsPanel : MonoBehaviour, IGameplayBaseEvents, IGameplayLifecycleEvents
	{
		[SerializeField] private TMP_Text _amountText;
		[SerializeField] private TMP_Text _questionText;
		[SerializeField] private Timer _timer;

		private int _totalQuestions;
		
		private readonly int _countdownDuration = 8;

		private void Start()
		{
			GameplayEventDispatcher.Instance.RegisterGameplayEvents(this);
		}

		public void OnGameplayStarted()
		{
			_totalQuestions = GamePlayManager.Instance.TotalQuestionsAmount;
			
			SetQuestion(GamePlayManager.Instance.QuestionIndex);
			
			_timer.OnTimerEnd += TimerOnOnTimerEnd;
			_timer.Initialize(_countdownDuration);
		}

		public void OnGameplayStopped()
		{
			_timer.OnTimerEnd -= TimerOnOnTimerEnd;
		}

		public void SetupQuestionPanel(int questionIndex)
		{
			_timer.OnTimerEnd += TimerOnOnTimerEnd;
			_timer.Initialize(_countdownDuration);
			
			SetQuestion(questionIndex);
		}
		
		private void TimerOnOnTimerEnd()
		{
			_timer.OnTimerEnd -= TimerOnOnTimerEnd;
			GamePlayManager.Instance.ChangeGamePlayScreen(GameplayScreenState.EndRound);
		}

		private void SetQuestion(int questionIndex)
		{
			_amountText.SetText($"{questionIndex + 1} / {_totalQuestions}");

			var question = GamePlayManager.Instance.GetQuestion(questionIndex);
			_questionText.SetText($"{question}");
		}
	}
}