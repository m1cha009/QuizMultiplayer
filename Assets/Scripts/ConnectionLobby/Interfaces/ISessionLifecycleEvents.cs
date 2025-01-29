namespace Quiz.Interfaces
{
	public interface ISessionLifecycleEvents
	{
		void OnSessionJoined();
		void OnSessionLeft();
	}
}