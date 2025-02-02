using UnityEngine;

namespace Quiz
{
	public abstract class BaseSession : MonoBehaviour
	{
		protected virtual void OnEnable()
		{
			SessionEventsDispatcher.Instance.RegisterBaseClassEvents(this);
		}
	}
}