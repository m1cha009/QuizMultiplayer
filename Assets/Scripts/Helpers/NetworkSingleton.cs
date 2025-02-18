using Unity.Netcode;
using UnityEngine;

namespace Quiz
{
	public abstract class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
	{
		public static T Instance { get; private set; }

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Debug.Log("Instance already exists");
				
				Destroy(this);
			}
			else
			{
				Instance = this as T;
			}
		}
	}
}