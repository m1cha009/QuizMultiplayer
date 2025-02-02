namespace Quiz
{
	public interface ISessionEvents
	{
		public void OnPlayerJoined(string playerId);
		public void OnPlayerLeft(string playerId);
	}
}