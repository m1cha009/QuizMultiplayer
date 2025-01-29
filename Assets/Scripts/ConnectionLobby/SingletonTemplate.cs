using UnityEngine;

namespace Quiz
{
	public abstract class SingletonTemplate<T> : MonoBehaviour where T: MonoBehaviour
	{
		private static T _instance;

		// public static T Instance => _instance;
		//
		// private void Awake()
		// {
		// 	Debug.Log("a");
		// 	if (_instance != null && _instance != this as T)
		// 	{
		// 		Debug.Log("b");
		// 		Destroy(_instance);
		// 	}
		// 	else
		// 	{
		// 		Debug.Log("c");
		// 		_instance = this as T;
		// 	}
		// }
		
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
			_instance = gameObject.AddComponent<T>();
			DontDestroyOnLoad(gameObject);
		}
	}
}