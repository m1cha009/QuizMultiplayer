using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Quiz
{
	public class QuestionsService
	{
		private string apiUrl = "http://3.67.207.205/questions";
		private string bearerToken = "uhX0w8bs85xAeXwf0wrXIRqkTuEKvR1d4V1RgLGHcrDFt5f490pzpywzwVEtdY9a";

		public async UniTask<QuestionData> GetQuestionData(CancellationToken cancellationToken = default)
		{
			try
			{
				using var request = UnityWebRequest.Get(apiUrl);
				request.SetRequestHeader("Authorization", "Bearer " + bearerToken);
				request.SetRequestHeader("Accept", "application/json");
				request.SetRequestHeader("User-Agent", "UnityGameClient/1.0");

				await request.SendWebRequest().WithCancellation(cancellationToken);

				if (request.result != UnityWebRequest.Result.Success)
				{
					Debug.Log($"Failed to fetch questions: {request.error}");
					SystemLogger.Log($"Failed to fetch questions: {request.error}");
				}

				var jsonResponse = request.downloadHandler.text;
				var questionData = JsonUtility.FromJson<QuestionData>(jsonResponse);

				if (questionData.questions == null)
				{
					Debug.Log("Failed to parse questions data");
					SystemLogger.Log("Failed to parse questions data");
				}

				return questionData;
			}
			catch (OperationCanceledException)
			{
				Debug.Log("Question fetch cancelled");
				SystemLogger.Log("Question fetch cancelled");
			}
			catch (Exception e)
			{
				Debug.LogError($"Error fetching questions: {e.Message}");
				SystemLogger.Log($"Error fetching questions: {e.Message}");
			}

			return default;
		}
	}
}