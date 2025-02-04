using UnityEngine;

namespace Quiz
{
	public class GameScreen : GameScreenFactory
	{
		[SerializeField] private PlayerListPanel _playerListPanel;

		public override void Enable()
		{
			base.Enable();
			
			_playerListPanel.InitializePlayers();
		}
	}
}