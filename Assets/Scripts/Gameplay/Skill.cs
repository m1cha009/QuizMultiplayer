using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quiz
{
	public class Skill : MonoBehaviour
	{
		[SerializeField] private SkillType _skillType;
		[SerializeField] private Button _skillButton;
		[SerializeField] private TMP_Text _priceText;
		[SerializeField] private TMP_Text _skillNameText;

		private ButtonEventsHandler _buttonEventsHandler;

		public SkillType SkillType => _skillType;

		public event Action<SkillType> SelectSkillEvent;

		private void Awake()
		{
			if (_skillType == SkillType.None)
			{
				_skillNameText.SetText(string.Empty);
				_priceText.SetText(string.Empty);
				_skillButton.interactable = false;
				gameObject.SetActive(false);
				
				return;
			}
			
			_buttonEventsHandler = _skillButton.GetComponent<ButtonEventsHandler>();
			
			_skillButton.onClick.AddListener(OnSkillClicked);
		}

		private void OnDestroy()
		{
			if (_skillButton == null)
			{
				return;
			}
			
			_skillButton.onClick.RemoveListener(OnSkillClicked);
		}

		public void Enable()
		{
			_skillButton.interactable = true;
			_buttonEventsHandler.TriggerTooltipActivity(false);
		}

		public void Disable()
		{
			_skillButton.interactable = false;
			_buttonEventsHandler.TriggerTooltipActivity(true);
		}
		
		public void SetupSkill(SkillData skillData, Tooltip tooltip)
		{
			SetNameText(skillData.SkillName);
			SetPriceText(skillData.Price);
			
			if (_buttonEventsHandler == null)
			{
				return;
			}
			
			_buttonEventsHandler.SetTooltip(tooltip);
		}

		public void SetNameText(string skillName) => _skillNameText.SetText(skillName);
		private void SetPriceText(int price)
		{
			_priceText.SetText($"€{price}");
		}

		private void OnSkillClicked()
		{
			SelectSkillEvent?.Invoke(_skillType);
			
			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}