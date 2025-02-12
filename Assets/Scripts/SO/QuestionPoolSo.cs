using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quiz
{
	[CreateAssetMenu(fileName = "Questions Pool", menuName = "SO/Questions Pool", order = 0)]
	public class QuestionPoolSo : ScriptableObject
	{
		[SerializeField] private QuestionData _questionsList = new();
		
		public List<Question> QuestionsList => _questionsList.questions.ToList();
	}
}