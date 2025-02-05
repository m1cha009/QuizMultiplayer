using UnityEngine;

namespace Quiz
{
	public class GameScreen : GameScreenFactory
	{
		[SerializeField] private PlayerListPanel _playerListPanel;
		[SerializeField] private QuestionsPanel _questionsPanel;

		public override void Enable()
		{
			base.Enable();
			
			_playerListPanel.InitializePlayers();
			_questionsPanel.InitializeQuestions(15);
		}
	}
}