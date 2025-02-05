using UnityEngine;

namespace Quiz
{
	public abstract class GameScreenFactory : MonoBehaviour
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