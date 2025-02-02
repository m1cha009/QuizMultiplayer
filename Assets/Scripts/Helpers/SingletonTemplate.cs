using UnityEngine;

namespace Quiz
{
	public abstract class SingletonTemplate<T> : MonoBehaviour where T: MonoBehaviour
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
			_instance = gameObject.AddComponent<T>();
			DontDestroyOnLoad(gameObject);
		}
	}
}