using UnityEngine;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;

namespace Quiz
{
	public class Skill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
	{
		[SerializeField] private int _price;
		[SerializeField] private Button _button;

		private Tooltip _tooltip;

		private void Awake()
		{
			_button.onClick.AddListener(OnSkillClicked);
		}

		private void OnDestroy()
		{
			_button.onClick.RemoveListener(OnSkillClicked);
		}
		
		
		public void SetupSkill(Tooltip tooltip)
		{
			_tooltip = tooltip;
		}

		private void OnSkillClicked()
		{
			// throw new NotImplementedException();
		}

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
				transform.parent as RectTransform,
				eventData.position,
				eventData.pressEventCamera,
				out var localPosition
			);

			((RectTransform)_tooltip.transform).anchoredPosition = localPosition + _tooltip.TooltipOffset;
		}
	}
}