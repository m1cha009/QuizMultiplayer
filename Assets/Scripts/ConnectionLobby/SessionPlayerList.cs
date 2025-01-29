using Quiz.Interfaces;
using UnityEngine;

namespace Quiz
{
	public class SessionPlayerList : BaseSession, ISessionEvents
	{
		[SerializeField] private SessionPlayerItem _sessionPlayerItemPrefab;
		[SerializeField] private Transform _parentTransform;

		public void OnPlayerJoined(string playerId)
		{
			Debug.Log($"Player Joined {playerId}");
		}
	}
}