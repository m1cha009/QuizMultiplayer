using System.Collections.Generic;
using Quiz.Interfaces;
using Unity.Services.Multiplayer;

namespace Quiz
{
	public class SessionEventsDispatcher : SingletonTemplate<SessionEventsDispatcher>
	{
		private readonly List<ISessionProvider> _sessionProviders = new();
		private readonly List<ISessionLifecycleEvents> _sessionLifecycles = new();
		private readonly List<ISessionEvents> _sessionEvents = new();
		private readonly List<IPlayerNameEvents> _playerNameEvents = new();
		
		private ISession _currentSession;
		
		public void RegisterBaseClassEvents(BaseSession baseSession)
		{
			if (baseSession is ISessionProvider sessionProvider)
			{
				sessionProvider.Session = _currentSession;
				_sessionProviders.Add(sessionProvider);
			}
			
			if (baseSession is ISessionLifecycleEvents sessionLifecycle)
			{
				_sessionLifecycles.Add(sessionLifecycle);
			}
			
			if (baseSession is ISessionEvents sessionEvents)
			{
				_sessionEvents.Add(sessionEvents);
			}

			if (baseSession is IPlayerNameEvents playerNameEvents)
			{
				_playerNameEvents.Add(playerNameEvents);
			}

		}
		
		public void UnRegisterBaseClassEvents(BaseSession baseSession)
		{
			if (baseSession is ISessionProvider sessionProvider)
			{
				_sessionProviders.Remove(sessionProvider);
			}
			
			if (baseSession is ISessionLifecycleEvents sessionLifecycle)
			{
				_sessionLifecycles.Remove(sessionLifecycle);
			}
			
			if (baseSession is ISessionEvents sessionEvents)
			{
				_sessionEvents.Remove(sessionEvents);
			}
			
			if (baseSession is IPlayerNameEvents playerNameEvents)
			{
				_playerNameEvents.Remove(playerNameEvents);
			}
		}

		public void OnSessionJoined(ISession session)
		{
			_currentSession = session;
			
			foreach (var sessionProvider in _sessionProviders)
			{
				sessionProvider.Session = _currentSession;
			}
			
			foreach (var sessionLifecycle in _sessionLifecycles)
			{
				sessionLifecycle.OnSessionJoined();
			}
		}
		
		public void OnSessionLeft()
		{
			foreach (var sessionLifecycle in _sessionLifecycles)
			{
				sessionLifecycle.OnSessionLeft();
			}
		}

		public void OnPlayerJoined(string playerId)
		{
			foreach (var sessionEvent in _sessionEvents)
			{
				sessionEvent.OnPlayerJoined(playerId);
			}
		}

		public void OnPlayerLeft(string playerId)
		{
			foreach (var sessionEvent in _sessionEvents)
			{
				sessionEvent.OnPlayerLeft(playerId);
			}
		}

		public void OnPlayerChangeName(string newName)
		{
			foreach (var playerNameEvent in _playerNameEvents)
			{
				playerNameEvent.OnPlayerNameChange(newName);
			}
		}


	}
}