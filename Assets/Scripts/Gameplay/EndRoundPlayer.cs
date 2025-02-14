using TMPro;
using UnityEngine;

namespace Quiz
{
	public class EndRoundPlayer : MonoBehaviour
	{
		[SerializeField] private TMP_Text _playerName;
		[SerializeField] private TMP_Text _playerAnswer;
		[SerializeField] private TMP_Text _answerPoints;
		[SerializeField] private TMP_Text _pointsAfterSkills;
		[SerializeField] private TMP_Text _totalPoints;

		public void Setup(PlayerData playerData)
		{
			_playerName.SetText(playerData.PlayerName);
			_playerAnswer.SetText(playerData.Answer);
			_answerPoints.SetText($"+{playerData.AnswerPoints.ToString()}");
			_pointsAfterSkills.SetText($"{playerData.SkillPoints.ToString()}");
			_totalPoints.SetText(playerData.TotalPoints.ToString());
		}
	}
}