using UnityEngine;

namespace Quiz
{
	public class SkillsManager : MonoBehaviour
	{
		[SerializeField] private Tooltip _tooltip;
		[SerializeField] private Skill _skillX2;

		private void Awake()
		{
			_tooltip.Hide();
		}

		private void Start()
		{
			_skillX2.SetupSkill(_tooltip);
		}
	}
}