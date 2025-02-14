using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;

namespace Quiz
{
	public class Skill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
	{
		[SerializeField] private int _price;
		[SerializeField] private SkillType _skillType;
		[SerializeField] private Button _skillButton;
		[SerializeField] private TMP_Text _priceText;

		private Tooltip _tooltip;

		private void Awake()
		{
			_skillButton.onClick.AddListener(OnSkillClicked);
			_priceText.SetText($"{_price}");
		}

		private void OnDestroy()
		{
			_skillButton.onClick.RemoveListener(OnSkillClicked);
		}
		public void SetupSkill(Tooltip tooltip)
		{
			_tooltip = tooltip;
		}
		
		public SkillType GetSkillType() => _skillType;

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