using System;

namespace Quiz
{
	[Serializable]
	public struct QuestionData
	{
		public Question[] questions;
	}

	[Serializable]
	public struct Question
	{
		public string[] answers;
		public string category;
		public string language;
		public int points;
		public string question;
		public string[] wrong_answers;
	}
}