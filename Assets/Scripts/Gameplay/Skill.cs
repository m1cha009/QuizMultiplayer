using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Quiz
{
	public class Skill : MonoBehaviour
	{
		[SerializeField] private int _price;
		[SerializeField] private SkillType _skillType;
		[SerializeField] private Button _skillButton;
		[SerializeField] private TMP_Text _priceText;

		private ButtonEventsHandler _buttonEventsHandler;

		public event Action<SkillType> SelectSkillEvent;

		private void Awake()
		{
			_buttonEventsHandler = _skillButton.GetComponent<ButtonEventsHandler>();
			
			_skillButton.onClick.AddListener(OnSkillClicked);
			_priceText.SetText($"{_price}");
		}

		private void OnDestroy()
		{
			_skillButton.onClick.RemoveListener(OnSkillClicked);
		}
		public void SetupSkill(Tooltip tooltip)
		{
			_buttonEventsHandler.SetTooltip(tooltip);
		}
		
		public SkillType GetSkillType() => _skillType;

		private void OnSkillClicked()
		{
			SelectSkillEvent?.Invoke(_skillType);
		}
	}
}