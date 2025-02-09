using TMPro;
using UnityEngine;

namespace Quiz
{
	public class Timer : MonoBehaviour
	{
		[SerializeField] private TMP_Text _timerText;
		
		public void SetTimer(int time)
		{
			_timerText.SetText($"{time:F0} sec");
		}
	}
}