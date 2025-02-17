using System;
using Unity.Collections;
using Unity.Netcode;

namespace Quiz
{
	[Serializable]
	public class PlayerData : INetworkSerializable
	{
		private FixedString128Bytes _playerId;
		private FixedString128Bytes _playerName;
		private FixedString128Bytes _answer;
		private int _answerPoints;
		private int _totalPoints;
		private SkillType[] _skillTypes;
		private int _skillPoints;
		private int _skillPrice;
		private int _skillIndex;

		public string PlayerId
		{
			get => _playerId.Value;
			set => _playerId = value;
		}
		public string PlayerName
		{
			get => _playerName.Value;
			set => _playerName = value;
		}
		public string Answer
		{
			get => _answer.Value;
			set => _answer = value;
		}

		public int AnswerPoints
		{
			get => _answerPoints;
			set => _answerPoints = value;
		}

		public int TotalPoints
		{
			get => _totalPoints;
			set => _totalPoints = value;
		}
		
		public SkillType[] SkillTypes
		{
			get
			{
				if (_skillTypes == null)
				{
					_skillTypes = new SkillType[6];
				}
				
				return _skillTypes;
			}
			set => _skillTypes = value;
		}

		public int SkillPoints
		{
			get => _skillPoints;
			set => _skillPoints = value;
		}
		
		public int SkillPrice
		{
			get => _skillPrice;
			set => _skillPrice = value;
		}
		
		public int SkillIndex
		{
			get => _skillIndex;
			set => _skillIndex = value;
		}
		
		public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
		{
			serializer.SerializeValue(ref _playerId);
			serializer.SerializeValue(ref _playerName);
			serializer.SerializeValue(ref _answer);
			serializer.SerializeValue(ref _answerPoints);
			serializer.SerializeValue(ref _totalPoints);
			serializer.SerializeValue(ref _skillTypes);
			serializer.SerializeValue(ref _skillPoints);
			serializer.SerializeValue(ref _skillPrice);
			serializer.SerializeValue(ref _skillIndex);
		}

		public void AddSkillType(SkillType skillType)
		{
			_skillTypes ??= new SkillType[6];

			_skillTypes[_skillIndex] = skillType;
			_skillIndex++;
		}
	}
}