using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
	[CreateAssetMenu(fileName = "Questions Pool", menuName = "SO/Questions Pool", order = 0)]
	public class QuestionPoolSo : ScriptableObject
	{
		[SerializeField] private List<QuestionData> _questionPool = new();
		
		public List<QuestionData> QuestionPool => _questionPool;
	}
}