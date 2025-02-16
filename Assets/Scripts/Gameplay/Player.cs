using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Quiz
{
	public class Player : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField] private TMP_Text _nameText;
		[SerializeField] private TMP_Text _answerText;
		[SerializeField] private TMP_Text _pointsText;
		[SerializeField] private Image _skillTypeImage;
		[SerializeField] private Image _pickSkillTargetImage;
		[SerializeField] private Sprite[] _skillTypeSprites;

		public event Action<string, Player> PlayerClickEvent;
		private string _playerId;

		private void Awake()
		{
			_skillTypeImage.gameObject.SetActive(false);
		}
		
		public void SetPlayerId(string playerId) => _playerId = playerId;

		public void SetName(string name) => _nameText.text = name;
		public void SetAnswer(string answer) => _answerText.text = answer;
		public void SetTotalPoints(int points) => _pointsText.text = points.ToString();
		
		public void SetSkillType(bool isOn, SkillType skillType)
		{
			switch (skillType)
			{
				case SkillType.None:
					_skillTypeImage.sprite = null;
					break;
				case SkillType.X2:
					_skillTypeImage.sprite = _skillTypeSprites[0];
					break;
				case SkillType.Resist:
					_skillTypeImage.sprite = _skillTypeSprites[0];
					break;
				case SkillType.Reverse:
					_skillTypeImage.sprite = _skillTypeSprites[1];
					break;
			}
			
			_skillTypeImage.gameObject.SetActive(isOn);
		}
		
		public void SetSkillTargetColor(bool isDefault)
		{
			if (isDefault)
			{
				_pickSkillTargetImage.color = Color.white;
				return;
			}
			
			_pickSkillTargetImage.color = Color.grey;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			PlayerClickEvent?.Invoke(_playerId, this);
		}
	}
}