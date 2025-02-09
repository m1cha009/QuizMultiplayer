namespace Quiz
{
	public class GameScreen : BaseScreens
	{
		public override void Enable()
		{
			base.Enable();
			
			GameManager.Instance.InitializePlayersData();
			GameplayEventDispatcher.Instance.OnGameplayInitialized();
		}

		public override void Disable()
		{
			base.Disable();
			
			GameplayEventDispatcher.Instance.OnGameplayDeInitialized();
		}
	}
}