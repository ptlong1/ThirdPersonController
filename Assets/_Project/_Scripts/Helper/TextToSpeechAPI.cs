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
	public EnvironmentVariablesContainer environmentVariablesContainer;
	public  string url;
	public bool isMale;

	public string result;
	public TextToSpeechResponse response;
	public AudioClip clip;

	private void Awake() {
		// url = environmentVariablesContainer.environmentVariables.ttsUrl;
	}
	private static void AddTokenHeader(UnityWebRequest webRequest, string token)
	{
		if (!String.IsNullOrWhiteSpace(token))
		{
			webRequest.SetRequestHeader("Authorization", "Bearer " + token);
		}
		else
		{
			Debug.Log("Token empty");
		}
	}

	
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


	public IEnumerator PostAudio(string text, string token)
	{
		string postBody = GetJson(text, isMale);
		url = environmentVariablesContainer.environmentVariables.ttsUrl;
		using (var req = new UnityWebRequest(url, "POST"))
		{
			AddTokenHeader(req, token);
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

	[ContextMenu("Speak")]
	void ConvertToAudioClip(string audioContent)
	{
		byte[] receivedBytes = System.Convert.FromBase64String(audioContent);	

		float[] samples = ConvertByteToFloat(receivedBytes);
		int channels = 1; //Assuming audio is mono because microphone input usually is
		int sampleRate = 24000; //Assuming your samplerate is 44100 or change to 48000 or whatever is appropriate

		clip = AudioClip.Create("ClipName", samples.Length, channels, sampleRate, false);
		clip.SetData(samples, 0);
	}
	public IEnumerator CR_ConvertToAudioClip(string content, string token)
	{
		yield return PostAudio(content, token);
		ConvertToAudioClip(response.audioContent);
	}
	private static float[] ConvertByteToFloat(byte[] array) {
		float[] floatArr = new float[array.Length / 2];

		for (int i = 0; i < floatArr.Length; i++) {
			floatArr[i] = ((float) BitConverter.ToInt16(array, i * 2))/32768.0f;
		}

		return floatArr;
	}

}
