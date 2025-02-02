namespace Quiz
{
	public interface ISessionLifecycleEvents
	{
		void OnSessionJoined();
		void OnSessionLeft();
	}
}