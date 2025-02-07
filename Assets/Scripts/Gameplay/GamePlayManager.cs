using System.Linq;
using UnityEngine;

namespace Quiz
{
	public class GamePlayManager : MonoSingleton<GamePlayManager>
	{
		[SerializeField] private QuestionPoolSo _questionsPool;

		[SerializeField] private GameObject _gameplayObject;
		[SerializeField] private EndRoundManager _endRoundObject;
		[SerializeField] private QuestionsPanel _questionsPanel;
		
		public int TotalQuestionsAmount => _questionsPool.QuestionPool.Count;
		public int QuestionIndex { get; private set; }
		public string GetQuestion(int questionId) => _questionsPool.QuestionPool[questionId].Question;


		private GameObject _currentGamePlayScreen;

		private void Start()
		{
			_currentGamePlayScreen = _gameplayObject;
		}

		public void ChangeInnerScreens(InnerScreensType innerScreensType)
		{
			_currentGamePlayScreen.SetActive(false);
			
			switch (innerScreensType)
			{
				case InnerScreensType.None:
					break;
				case InnerScreensType.Gameplay:
					_questionsPanel.SetupQuestionPanel(QuestionIndex);
					
					_gameplayObject.SetActive(true);
					_currentGamePlayScreen = _gameplayObject;
					break;
				case InnerScreensType.EndRound:
					QuestionIndex++;
					if (QuestionIndex >= TotalQuestionsAmount)
					{
						GameManager.Instance.ChangeScreen(ScreensType.FinishScreen);
						return;
					}
					
					var playerData = GameManager.Instance.GetPlayersData();
					
					_endRoundObject.InitializePlayers(playerData.Values.ToList());
					_endRoundObject.gameObject.SetActive(true);
					_currentGamePlayScreen = _endRoundObject.gameObject;
					break;
			}
		}
	}
}