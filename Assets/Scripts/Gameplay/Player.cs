using TMPro;
using UnityEngine;

namespace Quiz
{
	public class Player : MonoBehaviour
	{
		[SerializeField] private TMP_Text _nameText;
		[SerializeField] private TMP_Text _answerText;
		[SerializeField] private TMP_Text _pointsText;
		
		public void SetName(string name) => _nameText.text = name;
		public void SetAnswer(string answer) => _answerText.text = answer;
		public void SetPoints(int points) => _pointsText.text = points.ToString();
	}
}