using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Quiz
{
	public abstract class LazyNetworkSingleton<T> : NetworkBehaviour where T: NetworkBehaviour
	{
		private static T _instance;
		
		public static T Instance
		{
			get
			{
				if (_instance == (Object)null || _instance.gameObject == null)
					CreateInstance();
				return _instance;
			}
		}
        
		static void CreateInstance()
		{
			var gameObject = new GameObject($"{typeof(T).Name}");
			gameObject.AddComponent<NetworkObject>();
			
			_instance = gameObject.AddComponent<T>();
			DontDestroyOnLoad(gameObject);
		}
	}
}