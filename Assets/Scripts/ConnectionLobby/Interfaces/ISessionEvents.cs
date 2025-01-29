namespace Quiz.Interfaces
{
	public interface ISessionEvents
	{
		public void OnPlayerJoined(string playerId);
		public void OnPlayerLeft(string playerId);
	}
}