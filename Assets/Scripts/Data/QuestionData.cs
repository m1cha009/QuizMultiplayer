using System;
using System.Collections.Generic;

namespace Quiz
{
	[Serializable]
	public struct QuestionData
	{
		public string Question;
		public List<string> CorrectAnswers;
		public List<string> IncorrectAnswers;
		public int CorrectAnswerPoints;
	}
}