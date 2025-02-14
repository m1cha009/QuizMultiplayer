using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
	public class SkillsManager : MonoBehaviour
	{
		[SerializeField] private Tooltip _tooltip;
		[SerializeField] private Skill _selectedSkill;
		[SerializeField] private List<Skill> _skills;

		private SkillType _selectedSkillType = SkillType.None;

		public bool IsSkillUsed { get; set; }
		public SkillType SelectedSkillType => _selectedSkillType;

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
			_selectedSkillType = SkillType.None;
			
			_selectedSkill.SetName(string.Empty);
			_selectedSkill.gameObject.SetActive(false);

			IsSkillUsed = false;
		}
		
		private void OnSkillSelected(SkillType skillType)
		{
			_selectedSkillType = skillType;
			
			_selectedSkill.SetName(_selectedSkillType.ToString());
			_selectedSkill.gameObject.SetActive(true);
		}
	}
}