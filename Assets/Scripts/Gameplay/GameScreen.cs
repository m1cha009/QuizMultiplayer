namespace Quiz
{
	public class GameScreen : BaseScreens
	{
		public override void Enable()
		{
			base.Enable();
			
			GameplayEventDispatcher.Instance.OnGameplayInitialized();
		}

		public override void Disable()
		{
			base.Disable();
			
			GameplayEventDispatcher.Instance.OnGameplayDeInitialized();
		}
	}
}