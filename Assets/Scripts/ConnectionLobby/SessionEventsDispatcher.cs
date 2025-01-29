using System.Collections.Generic;
using Quiz.Interfaces;
using Unity.Services.Multiplayer;

namespace Quiz
{
	public class SessionEventsDispatcher : SingletonTemplate<SessionEventsDispatcher>
	{
		private readonly List<ISessionLifecycleEvents> _sessionLifecycles = new();
		private readonly List<ISessionEvents> _sessionEvents = new();
		
		public void RegisterBaseClassEvents(BaseSession baseSession)
		{
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