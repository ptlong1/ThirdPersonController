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
	public TextAsset audioContent;
	
	private void Start() {
		audioSource = GetComponent<AudioSource>();
		UpdateCanvas();
		for(int i = 0; i < questions.Length; ++i)
		{
			Button newBtn = Instantiate(questionBtnPrefab, choices.transform);
			newBtn.GetComponentInChildren<TMP_Text>().text = questions[i];
			string answer = answers[i];
			newBtn.onClick.AddListener(() => SetHelper(answer));
		}
	}

	void SetHelper(string str)
	{
		helperAsk = str;
		UpdateCanvas();
	}
	void UpdateCanvas()
	{
		helper.text = helperAsk;
		Speak();
	}

	[ContextMenu("Speak")]
	void Speak()
	{
		byte[] receivedBytes = System.Convert.FromBase64String(audioContent.text);	
		Debug.Log(receivedBytes.Length);
		for (int i = 0; i < receivedBytes.Length; ++i)
			Debug.Log(receivedBytes[i]);
		float[] samples = new float[receivedBytes.Length / 4]; //size of a float is 4 bytes
		Buffer.BlockCopy(receivedBytes, 0, samples, 0, receivedBytes.Length);
		// Debug.Log(samples.Length);
		// for (int i = 0; i < samples.Length; ++i)
		// 	Debug.Log(samples[i]);
		int channels = 1; //Assuming audio is mono because microphone input usually is
		int sampleRate = 44100; //Assuming your samplerate is 44100 or change to 48000 or whatever is appropriate

		AudioClip clip = AudioClip.Create("ClipName", samples.Length, channels, sampleRate, false);
		clip.SetData(samples, 0);
		audioSource.clip = clip;
		audioSource.Play();
	}
}
