using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HelperConversation : MonoBehaviour
{
	public TMP_Text helper;
	public string helperAsk;
	
	public GameObject choices;
	public Button questionBtnPrefab;
	public string[] questions;
	public string[] answers;
	AudioSource audioSource;
	// public TextAsset audioContent;
	TextToSpeechAPI textToSpeechAPI;
	
	private void Awake() {
		audioSource = GetComponent<AudioSource>();
		textToSpeechAPI = GetComponent<TextToSpeechAPI>();
		
	}
	private void Start() {
		for(int i = 0; i < questions.Length; ++i)
		{
			Button newBtn = Instantiate(questionBtnPrefab, choices.transform);
			newBtn.GetComponentInChildren<TMP_Text>().text = questions[i];
			string answer = answers[i];
			newBtn.onClick.AddListener(() => SetHelper(answer));
		}
	}

	private void OnEnable() {
		UpdateCanvas();
	}

	void SetHelper(string str)
	{
		helperAsk = str;
		UpdateCanvas();
	}
	void UpdateCanvas()
	{
		helper.text = helperAsk;
		StartCoroutine(CR_Speak(helperAsk));
	}

	IEnumerator CR_Speak(string content)
	{
		yield return textToSpeechAPI.PostAudio(content);
		Speak(textToSpeechAPI.response.audioContent);
	}

	[ContextMenu("Speak")]
	void Speak(string audioContent)
	{
		byte[] receivedBytes = System.Convert.FromBase64String(audioContent);	
		// Debug.Log(receivedBytes.Length);
		// for (int i = 0; i < receivedBytes.Length; ++i)
			// Debug.Log(receivedBytes[i]);
		// float[] samples = new float[receivedBytes.Length / 4 + 1]; //size of a float is 4 bytes
		// Buffer.BlockCopy(receivedBytes, 0, samples, 0, receivedBytes.Length);
		// Debug.Log(samples.Length);
		// for (int i = 0; i < samples.Length; ++i)
		// 	Debug.Log(samples[i]);

		float[] samples = ConvertByteToFloat(receivedBytes);
		// Debug.Log(samples.Length);
		// for (int i = 0; i < 1000; ++i)
		// 	if (Mathf.Abs(samples[i]) > 1)
		// 	{
		// 		Debug.Log(samples[i]);
		// 	}
		int channels = 1; //Assuming audio is mono because microphone input usually is
		int sampleRate = 24000; //Assuming your samplerate is 44100 or change to 48000 or whatever is appropriate

		AudioClip clip = AudioClip.Create("ClipName", samples.Length, channels, sampleRate, false);
		clip.SetData(samples, 0);
		audioSource.clip = clip;
		audioSource.Play();
	}
	// private float[] ConvertByteToFloat(byte[] array)
    // {
    //     float[] floatArr = new float[array.Length / 4];
    //     for (int i = 0; i < floatArr.Length; i++)
    //     {
    //         if (BitConverter.IsLittleEndian)
    //         {
    //             Array.Reverse(array, i * 4, 4);
    //         }
    //         floatArr[i] = BitConverter.ToSingle(array, i * 4) / 0x80000000;
    //     }
    //     return floatArr;
    // }
	private static float[] ConvertByteToFloat(byte[] array) {
    float[] floatArr = new float[array.Length / 2];

    for (int i = 0; i < floatArr.Length; i++) {
        floatArr[i] = ((float) BitConverter.ToInt16(array, i * 2))/32768.0f;
    }

    return floatArr;
}


}
