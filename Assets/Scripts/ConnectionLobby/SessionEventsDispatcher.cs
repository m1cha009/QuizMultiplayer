using System.Collections.Generic;
using Quiz.Interfaces;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace Quiz
{
	public class SessionEventsDispatcher : SingletonTemplate<SessionEventsDispatcher>
	{
		private readonly List<ISessionProvider> _sessionProviders = new();
		private readonly List<ISessionLifecycleEvents> _sessionLifecycles = new();
		private readonly List<ISessionEvents> _sessionEvents = new();
		
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

		}
		
		public void UnRegisterBaseClassEvents(BaseSession baseSession)
		{
			if (baseSession is ISessionLifecycleEvents sessionLifecycle)
			{
				_sessionLifecycles.Remove(sessionLifecycle);
			}
			
			if (baseSession is ISessionEvents sessionEvents)
			{
				_sessionEvents.Remove(sessionEvents);
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

		public void OnPlayerJoined(string playerId)
		{
			foreach (var sessionEvent in _sessionEvents)
			{
				sessionEvent.OnPlayerJoined(playerId);
			}
		}

		public void OnSessionLeft()
		{
			
		}
	}
}