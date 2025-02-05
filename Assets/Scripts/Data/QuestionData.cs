using System;
using System.Collections.Generic;

namespace Quiz
{
	[Serializable]
	public class QuestionData
	{
		public string Question;
		public List<string> CorrectAnswers;
	}
}