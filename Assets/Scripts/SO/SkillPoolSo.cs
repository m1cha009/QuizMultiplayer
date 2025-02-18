using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
	[CreateAssetMenu(fileName = "Skill Pool", menuName = "SO/Skill Pool", order = 0)]
	public class SkillPoolSo : ScriptableObject
	{
		[SerializeField] private List<SkillData> _skillPool;

		public int GetSkillPrice(SkillType skillType)
		{
			for (var i = 0; i < _skillPool.Count; i++)
			{
				if (_skillPool[i].SkillType == skillType)
				{
					return _skillPool[i].Price;
				}
			}

			return -1;
		}

		public string GetSkillName(SkillType skillType)
		{
			for (var i = 0; i < _skillPool.Count; i++)
			{
				if (_skillPool[i].SkillType == skillType)
				{
					return _skillPool[i].SkillName;
				}
			}

			return string.Empty;
		}
	}
}