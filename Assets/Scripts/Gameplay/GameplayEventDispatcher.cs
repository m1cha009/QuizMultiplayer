using System.Collections.Generic;

namespace Quiz
{
	public class GameplayEventDispatcher : MonoSingleton<GameplayEventDispatcher>
	{
		private readonly List<IGameplayLifecycleEvents> _gameplayLifecycleEvents = new();
		
		public void RegisterGameplayEvents(IGameplayBaseEvents gameplayBaseEvents)
		{
			if (gameplayBaseEvents is IGameplayLifecycleEvents gameplayLifecycleEvents)
			{
				_gameplayLifecycleEvents.Add(gameplayLifecycleEvents);
			}
		}

		public void OnGameplayInitialized()
		{
			foreach (var gameplayLifecycle in _gameplayLifecycleEvents)
			{
				gameplayLifecycle.OnGameplayInitialized();
			}
		}
		
		public void OnGameplayDeInitialized()
		{
			foreach (var gameplayLifecycle in _gameplayLifecycleEvents)
			{
				gameplayLifecycle.OnGameplayDeInitialized();
			}
		}
	}
}