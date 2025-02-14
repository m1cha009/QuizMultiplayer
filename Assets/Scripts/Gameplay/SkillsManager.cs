using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
	public class SkillsManager : MonoBehaviour
	{
		[SerializeField] private Tooltip _tooltip;
		[SerializeField] private List<Skill> _skills;

		private SkillType _selectedSkill = SkillType.None;

		public SkillType SelectedSkill => _selectedSkill;

		private void Awake()
		{
			_tooltip.Hide();
		}

		private void Start()
		{
			foreach (var skill in _skills)
			{
				skill.SelectSkillEvent += OnSkillSelected;
				skill.SetupSkill(_tooltip);
			}
		}

		private void OnDestroy()
		{
			foreach (var skill in _skills)
			{
				skill.SelectSkillEvent -= OnSkillSelected;
			}
		}

		public void ResetSkills()
		{
			_selectedSkill = SkillType.None;
		}
		
		private void OnSkillSelected(SkillType skillType)
		{
			_selectedSkill = skillType;
		}
	}
}