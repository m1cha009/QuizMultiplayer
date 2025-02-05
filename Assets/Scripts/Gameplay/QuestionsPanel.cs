using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Quiz
{
	public class QuestionsPanel : NetworkBehaviour, IGameplayBaseEvents, IGameplayLifecycleEvents
	{
		[SerializeField] private TMP_Text _amountText;
		[SerializeField] private TMP_Text _questionText;
		[SerializeField] private TMP_Text _timerText;

		private int _totalQuestions;
		private int _questionIndex = 0;
		private GameplayManager _gameplayManager;
		
		private readonly int _countdownDuration = 5;
		private readonly NetworkVariable<int> _serverTimeLeft = new();
		private readonly float _syncInterval = 1f;
		private float _lastSyncTime;
		private float _localTimeLeft;

		private readonly NetworkVariable<int> _serverQuestionIndex = new();

		private bool _isGameplayStarted;

		private void Start()
		{
			GameplayEventDispatcher.Instance.RegisterGameplayEvents(this);
		}

		public void OnGameplayStarted()
		{
			if (_gameplayManager == null)
			{
				_gameplayManager = GameplayManager.Instance;
			}
			
			_totalQuestions = _gameplayManager.TotalQuestionsAmount;
			
			DisplayQuestion(_questionIndex);

			_localTimeLeft = _countdownDuration;
			_serverTimeLeft.OnValueChanged += OnTimerValueChanged;
			_serverQuestionIndex.OnValueChanged += OnQuestionChanged;
			_isGameplayStarted = true;
		}

		public void OnGameplayStopped()
		{
			_serverTimeLeft.OnValueChanged -= OnTimerValueChanged;
			_serverQuestionIndex.OnValueChanged -= OnQuestionChanged;
			_isGameplayStarted = false;
		}
		
		private void OnTimerValueChanged(int previousValue, int newValue)
		{
			SetTimer(newValue);
		}
		
		private void OnQuestionChanged(int previousValue, int newValue)
		{
			DisplayQuestion(newValue);
		}

		private void Update()
		{
			if (!_isGameplayStarted || !IsServer) return;
			
			if (_localTimeLeft > 0)
			{
				_localTimeLeft -= Time.deltaTime;
			
				if (Time.time - _lastSyncTime >= _syncInterval)
				{
					_lastSyncTime = Time.time;
					_serverTimeLeft.Value = Mathf.FloorToInt(_localTimeLeft); // it is going to call OnTimerValueChanged
				}
			}
			else
			{
				_questionIndex = (_questionIndex + 1) % _totalQuestions;
				_serverQuestionIndex.Value = _questionIndex; // it is going to call OnQuestionChanged
				_localTimeLeft = _countdownDuration;
			}
		}

		private void SetTimer(int time)
		{
			_timerText.SetText($"{time:F0} sec");
		}

		private void DisplayQuestion(int questionIndex)
		{
			_amountText.SetText($"{questionIndex + 1} / {_totalQuestions}");

			var question = _gameplayManager.GetQuestion(questionIndex);
			_questionText.SetText($"{question}");
		}
	}
}