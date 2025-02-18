using UnityEngine;

namespace Quiz
{
	public abstract class BaseSession : MonoBehaviour, IBaseSession
	{
		protected virtual void OnEnable()
		{
			SessionEventsDispatcher.Instance.RegisterBaseClassEvents(this);
		}
	}
}