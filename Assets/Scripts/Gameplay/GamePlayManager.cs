using UnityEngine;

namespace Quiz
{
	public class GamePlayManager : NetworkSingleton<GamePlayManager>, IGameplayBaseEvents, IGameplayLifecycleEvents
	{
		[SerializeField] private QuestionPoolSo _questionsPool;

		[SerializeField] private GameObject _gameplayObject;
		[SerializeField] private EndRoundManager _endRoundObject;
		[SerializeField] private QuestionsPanel _questionsPanel;
		
		public int TotalQuestionsAmount => _questionsPool.QuestionPool.Count;
		public int QuestionIndex { get; private set; }
		public string GetQuestion(int questionId) => _questionsPool.QuestionPool[questionId].Question;


		private GameplayScreenState _gameplayScreenState = GameplayScreenState.None;
		private GameObject _currentGamePlayScreen;

		private void Start()
		{
			GameplayEventDispatcher.Instance.RegisterGameplayEvents(this);
		}

		public void OnGameplayStarted()
		{
			_gameplayScreenState = GameplayScreenState.Gameplay;
			_currentGamePlayScreen = _gameplayObject;
		}

		public void OnGameplayStopped()
		{
			_gameplayScreenState = GameplayScreenState.None;
		}

		public void ChangeGamePlayScreen(GameplayScreenState gameplayScreenState)
		{
			_currentGamePlayScreen.SetActive(false);
			
			switch (gameplayScreenState)
			{
				case GameplayScreenState.None:
					break;
				case GameplayScreenState.Gameplay:
					_questionsPanel.SetupQuestionPanel(QuestionIndex);
					
					_gameplayObject.SetActive(true);
					_currentGamePlayScreen = _gameplayObject;
					break;
				case GameplayScreenState.EndRound:
					var playerData = GameManager.Instance.GetPlayersData();
					
					_endRoundObject.InitializePlayers(playerData);
					_endRoundObject.gameObject.SetActive(true);
					_currentGamePlayScreen = _endRoundObject.gameObject;
					QuestionIndex++;
					break;
			}
		}
	}
}