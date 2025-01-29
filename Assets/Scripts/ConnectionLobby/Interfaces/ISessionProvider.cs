using Unity.Services.Multiplayer;

namespace Quiz.Interfaces
{
	public interface ISessionProvider
	{
		public ISession Session { get; set; }
	}
}