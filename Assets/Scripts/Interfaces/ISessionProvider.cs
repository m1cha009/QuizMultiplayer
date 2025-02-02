using Unity.Services.Multiplayer;

namespace Quiz
{
	public interface ISessionProvider
	{
		public ISession Session { get; set; }
	}
}