using Unity.Netcode;

namespace Quiz
{
	public abstract class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
	{
		public static T Instance { get; private set; }

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this as T;
			}
		}
	}
}