using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HelperConversation : MonoBehaviour
{
	public TMP_Text helper;
	public UserResponse userResponse;
	public string helperAsk;
	public string currentStr;
	
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
			newBtn.gameObject.SetActive(true);
			newBtn.GetComponentInChildren<TMP_Text>().text = questions[i];
			string answer = answers[i];
			newBtn.onClick.AddListener(() => SetHelper(answer));
		}
	}

	private void OnEnable() {
		currentStr = helperAsk;
		UpdateCanvas();
	}

	void SetHelper(string str)
	{
		currentStr = str;
		UpdateCanvas();
	}
	void UpdateCanvas()
	{
		helper.text = currentStr;
		StartCoroutine(CR_Speak(currentStr));
	}

	IEnumerator CR_Speak(string content)
	{
		yield return textToSpeechAPI.CR_ConvertToAudioClip(content, userResponse.token);
		Speak(textToSpeechAPI.clip);
	}

	[ContextMenu("Speak")]
	void Speak(AudioClip clip)
	{
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

}
