using TMPro;
using UnityEngine;

namespace Quiz
{
	public class Player : MonoBehaviour
	{
		[SerializeField] private TMP_Text _nameText;
		[SerializeField] private TMP_Text _answerText;
		
		public void SetName(string name) => _nameText.text = name;
		public void SetAnswer(string answer) => _answerText.text = answer;
	}
}