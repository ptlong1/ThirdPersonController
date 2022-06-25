using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[Serializable]
public class TextToSpeechResponse
{
	public string audioContent;
}

[Serializable]
public class TextToSpeechRequest
{
	public AudioConfig audioConfig;
	public TTSInput input;
	public TTSVoice voice;
}

[Serializable]
public class TextToSpeechAPI: MonoBehaviour
{
	public  string url;
	public bool isMale;

	public string result;
	public TextToSpeechResponse response;

	string GetJson(string text, bool isMale)
	{
		TextToSpeechRequest request = new TextToSpeechRequest();
		request.audioConfig = new AudioConfig();
		request.input = new TTSInput();
		request.voice = new TTSVoice();
		request.audioConfig.audioEncoding = "LINEAR16";
		request.audioConfig.pitch = 0;
		request.audioConfig.speakingRate = 1;
		request.input.text = text;
		request.voice.languageCode = "en-US";
		request.voice.name = isMale?"en-US-Wavenet-B":"en-US-Wavenet-F";
		return JsonUtility.ToJson(request);
	}


	public IEnumerator PostAudio(string text)
	{
		string postBody = GetJson(text, isMale);
		// Debug.Log(postBody);
		using (var req = new UnityWebRequest(url, "POST"))
		{
			byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(postBody);
			req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
			req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			req.SetRequestHeader("Content-Type", "application/json");

			yield return req.SendWebRequest();

			if (req.result != UnityWebRequest.Result.Success)
			{
				Debug.Log("Error While Sending: " + req.error);
				Debug.Log("Received: " + req.downloadHandler.text);
			}
			else
			{
				// Debug.Log("Received: " + req.downloadHandler.text);
				result = req.downloadHandler.text;
				response = JsonUtility.FromJson<TextToSpeechResponse>(result);
			}
		}
	}
}
