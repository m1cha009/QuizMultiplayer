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
		
		public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
		{
			serializer.SerializeValue(ref _playerId);
			serializer.SerializeValue(ref _playerName);
			serializer.SerializeValue(ref _answer);
			serializer.SerializeValue(ref _answerPoints);
			serializer.SerializeValue(ref _totalPoints);
		}
	}
}