namespace Quiz
{
	public struct PlayerData
	{
		public string PlayerId { get; set; }
		public string PlayerName { get; set; }
		public string Answer { get; set; }

		public int AnswerPoints { get; set; }

		public int TotalPoints { get; set; }
	}
}