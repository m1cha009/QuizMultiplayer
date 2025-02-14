using UnityEngine;
using UnityEngine.EventSystems;

namespace Quiz
{
	public class ButtonEventsHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
	{
		private Tooltip _tooltip;
		public void SetTooltip(Tooltip tooltip) => _tooltip = tooltip;
		
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (_tooltip == null) return;

			_tooltip.Show();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (_tooltip == null) return;
			
			_tooltip.Hide();
		}

		public void OnPointerMove(PointerEventData eventData)
		{
			if (_tooltip == null) return;
			
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				transform.parent.parent as RectTransform,
				eventData.position,
				eventData.pressEventCamera,
				out var localPosition
			);

			((RectTransform)_tooltip.transform).anchoredPosition = localPosition + _tooltip.TooltipOffset;
		}
	}
}