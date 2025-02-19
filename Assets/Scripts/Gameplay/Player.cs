namespace Quiz
{
	public class Player
	{
		private PlayerData _playerData;

		public Player(PlayerData playerData)
		{
			_playerData = playerData;
		}

		public PlayerData PlayerData => _playerData;

		public void ClearPlayer()
		{
			ClearPlayerData();
			ClearPlayerSkillData();
		}

		public void AddPLayerSkillData(PlayerSkillData playerSkillData)
		{
			_playerData.PlayerSkillsData.Add(playerSkillData);
		}
		
		private void ClearPlayerData()
		{
			_playerData.Answer = string.Empty;
			_playerData.AnswerPoints = 0;
		}
		
		private void ClearPlayerSkillData() => _playerData.PlayerSkillsData.Clear();
	}
}