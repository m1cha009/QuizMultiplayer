using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quiz
{
	public class Skill : MonoBehaviour
	{
		[SerializeField] private int _price;
		[SerializeField] private SkillType _skillType;
		[SerializeField] private Button _skillButton;
		[SerializeField] private TMP_Text _priceText;
		[SerializeField] private TMP_Text _skillNameText;

		private ButtonEventsHandler _buttonEventsHandler;

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
			
			_priceText.SetText($"{_price}");
			
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
		
		public void SetName(string name) => _skillNameText.SetText(name);
		
		public void SetupSkill(Tooltip tooltip)
		{
			if (_buttonEventsHandler == null)
			{
				return;
			}
			
			_buttonEventsHandler.SetTooltip(tooltip);
		}

		private void OnSkillClicked()
		{
			SelectSkillEvent?.Invoke(_skillType);
			
			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}