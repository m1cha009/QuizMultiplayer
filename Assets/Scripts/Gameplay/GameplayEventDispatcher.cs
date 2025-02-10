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

		public void OnGameplayStarted()
		{
			foreach (var gameplayLifecycle in _gameplayLifecycleEvents)
			{
				gameplayLifecycle.OnGameplayStarted();
			}
		}
		
		public void OnGameplayStopped()
		{
			foreach (var gameplayLifecycle in _gameplayLifecycleEvents)
			{
				gameplayLifecycle.OnGameplayStopped();
			}
		}
	}
}