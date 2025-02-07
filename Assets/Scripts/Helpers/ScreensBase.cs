using UnityEngine;

namespace Quiz
{
	public abstract class BaseScreens : MonoBehaviour
	{
		public virtual void Enable()
		{
			gameObject.SetActive(true);
		}
		
		public virtual void Disable()
		{
			gameObject.SetActive(false);
		}
	}
}