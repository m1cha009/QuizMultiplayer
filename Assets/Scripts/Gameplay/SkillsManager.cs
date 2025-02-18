using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
	public class SkillsManager : MonoBehaviour, IGameplayBaseEvents , IGameplayLifecycleEvents
	{
		[SerializeField] private Tooltip _tooltip;
		[SerializeField] private Skill _selectedSkill;
		[SerializeField] private List<Skill> _skills;
		[SerializeField] private SkillPoolSo _skillPoolSo;

		private SkillType _selectedSkillType;

		public bool IsSkillUsed { get; set; }
		public SkillType SelectedSkillType => _selectedSkillType;

		public int SelectedSkillPrice => _skillPoolSo.GetSkillPrice(_selectedSkillType);

		private void Awake()
		{
			_tooltip.Hide();
			
			GameplayEventDispatcher.Instance.RegisterGameplayEvents(this);
		}

		private void Start()
		{
			var skillData = new SkillData();
			
			foreach (var skill in _skills)
			{
				skill.SelectSkillEvent += OnSkillSelected;
				
				var skillType = skill.SkillType;
				var skillPrice = _skillPoolSo.GetSkillPrice(skillType);
				var skillName = _skillPoolSo.GetSkillName(skillType);

				skillData.SkillName = skillName;
				skillData.Price = skillPrice;
				
				skill.SetupSkill(skillData, _tooltip);
			}
		}

		private void OnDestroy()
		{
			foreach (var skill in _skills)
			{
				skill.SelectSkillEvent -= OnSkillSelected;
			}
		}
		
		public void OnGameplayStarted()
		{
			ResetSkills();
		}

		public void OnGameplayStopped() { }

		public void ResetSkills()
		{
			_selectedSkillType = SkillType.None;
			
			_selectedSkill.SetNameText(string.Empty);
			_selectedSkill.gameObject.SetActive(false);

			var playerId = GameManager.Instance.CurrentPlayerId;
			var playerData = GameManager.Instance.GetPlayerData(playerId);
			
			foreach (var skill in _skills)
			{
				var skillPrice = _skillPoolSo.GetSkillPrice(skill.SkillType);
				if (skillPrice > playerData.TotalPoints)
				{
					skill.Disable();
				}
				else
				{
					skill.Enable();
				}
			}

			IsSkillUsed = false;
		}
		
		private void OnSkillSelected(SkillType skillType)
		{
			_selectedSkillType = skillType;
			
			var skillName = _skillPoolSo.GetSkillName(skillType);
			
			_selectedSkill.SetNameText(skillName);
			_selectedSkill.gameObject.SetActive(true);
		}


	}
}