using Unity.Netcode;

namespace Quiz
{
	public abstract class NetworkBaseSession : NetworkBehaviour, IBaseSession
	{
		protected virtual void OnEnable()
		{
			SessionEventsDispatcher.Instance.RegisterBaseClassEvents(this);
		}
	}
}