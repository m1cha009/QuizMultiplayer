using System;
using Unity.Collections;
using Unity.Netcode;

namespace Quiz
{
	[Serializable]
	public struct NetworkPlayerData : INetworkSerializable
	{
		public FixedString128Bytes PlayerId;
		public FixedString128Bytes PlayerName;
		public FixedString128Bytes PlayerAnswer;
		public int AnswerPoints;
		public int SkillPoints;
		public int TotalPoints;
		public NetworkPLayerSkillData[] PLayerSkillsData;

		public NetworkPlayerData(PlayerData playerData)
		{
			PlayerId = playerData.PlayerId;
			PlayerName = playerData.PlayerName;
			PlayerAnswer = playerData.Answer;
			AnswerPoints = playerData.AnswerPoints;
			SkillPoints = playerData.SkillPoints;
			TotalPoints = playerData.TotalPoints;

			PLayerSkillsData = new NetworkPLayerSkillData[playerData.PlayerSkillsData.Count];
			var n = 0;
			foreach (var playerSkillData in playerData.PlayerSkillsData)
			{
				var playerNetworkSkillData = new NetworkPLayerSkillData(playerSkillData);
				PLayerSkillsData[n] = playerNetworkSkillData;
			}
		}
		
		public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
		{
			serializer.SerializeValue(ref PlayerId);
			serializer.SerializeValue(ref PlayerName);
			serializer.SerializeValue(ref PlayerAnswer);
			serializer.SerializeValue(ref AnswerPoints);
			serializer.SerializeValue(ref SkillPoints);
			serializer.SerializeValue(ref TotalPoints);

			var playerSkillsLenght = 0;
			if (serializer.IsWriter)
			{
				playerSkillsLenght = PLayerSkillsData?.Length ?? 0;
			}
			
			serializer.SerializeValue(ref playerSkillsLenght);

			if (serializer.IsReader)
			{
				PLayerSkillsData = new NetworkPLayerSkillData[playerSkillsLenght];
			}

			for (var i = 0; i < playerSkillsLenght; i++)
			{
				if (serializer.IsReader)
				{
					var temp = new NetworkPLayerSkillData();
					temp.NetworkSerialize(serializer);
					PLayerSkillsData[i] = temp;
				}
				else
				{
					var temp = PLayerSkillsData[i];
					temp.NetworkSerialize(serializer);
				}
			}
		}
	}
}