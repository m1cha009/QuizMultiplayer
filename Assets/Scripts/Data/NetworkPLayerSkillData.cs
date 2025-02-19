using System;
using Unity.Collections;
using Unity.Netcode;

namespace Quiz
{
	[Serializable]
	public struct NetworkPLayerSkillData : INetworkSerializable
	{
		public SkillType SkillTypes;
		public FixedString128Bytes AttackerId;
		public FixedString128Bytes AttackerName;
		public int SkillPrice;

		public NetworkPLayerSkillData(PlayerSkillData skillData)
		{
			SkillTypes = skillData.SkillType;
			AttackerId = skillData.AttackerId;
			AttackerName = skillData.AttackerName;
			SkillPrice = skillData.SkillPrice;
		}
		
		public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
		{
			serializer.SerializeValue(ref SkillTypes);
			serializer.SerializeValue(ref AttackerId);
			serializer.SerializeValue(ref AttackerName);
			serializer.SerializeValue(ref SkillPrice);
		}
	}
}