using System;
using System.Collections.Generic;

namespace Quiz
{
	[Serializable]
	public struct QuestionData
	{
		public List<Question> questions;
	}

	[Serializable]
	public struct Question
	{
		public List<string> answers;
		public string category;
		public string language;
		public int points;
		public string question;
		public List<string> wrong_answers;
	}
}