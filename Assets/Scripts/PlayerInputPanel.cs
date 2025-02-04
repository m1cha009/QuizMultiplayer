using System;
using TMPro;
using UnityEngine;

namespace Quiz
{
	public class PlayerInputPanel : MonoBehaviour
	{
		[SerializeField] private TMP_InputField _inputField;

		private void Awake()
		{
			_inputField.onEndEdit.AddListener(OnEndEdit);
		}

		private void OnEndEdit(string text)
		{
			Debug.Log($"New Text {text}");
		}
	}
}