namespace Quiz
{
	public class GameScreen : GameScreenFactory
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