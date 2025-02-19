using System;
using System.Collections.Generic;

namespace Quiz
{
	[Serializable]
	public class PlayerData
	{
		public PlayerData() {}
		
		public PlayerData(NetworkPlayerData networkPlayerData)
		{
			PlayerId = networkPlayerData.PlayerId.ToString();
			PlayerName = networkPlayerData.PlayerName.ToString();
			Answer = networkPlayerData.PlayerAnswer.ToString();
			AnswerPoints = networkPlayerData.AnswerPoints;
			TotalPoints = networkPlayerData.TotalPoints;
			
			PlayerSkillsData.Clear();
			foreach (var networkPLayerSkillData in networkPlayerData.PLayerSkillsData)
			{
				PlayerSkillsData.Add(new PlayerSkillData()
				{
					SkillType = networkPLayerSkillData.SkillTypes,
					AttackerId = networkPLayerSkillData.AttackerId.ToString(),
					AttackerName = networkPLayerSkillData.AttackerName.ToString(),
					SkillPrice = networkPLayerSkillData.SkillPrice
				});
			}
		}

		public string PlayerId;
		public string PlayerName;
		public string Answer;
		public int AnswerPoints;
		public int SkillPoints;
		public int TotalPoints;
		public List<PlayerSkillData> PlayerSkillsData = new();
	}
}