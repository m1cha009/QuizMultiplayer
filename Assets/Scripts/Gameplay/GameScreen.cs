using UnityEngine;

namespace Quiz
{
	public class GameScreen : GameScreenFactory
	{
		[SerializeField] private PlayerListPanel _playerListPanel;
		[SerializeField] private PlayerInputPanel _playerInputPanel;

		public override void Enable()
		{
			base.Enable();
			
			_playerListPanel.InitializePlayers();
		}
	}
}