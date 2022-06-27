using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using ScriptableObjectArchitecture;

public class NPCController : MonoBehaviour
{
	public UserResponse userResponse;
	TextToSpeechAPI textToSpeechAPI;
	AudioSource audioSource;
	public string welcomeSentence;
	public float repeatAfter;
	public HelperConversation canvasHelper;
	Animator animator;
	public GameEvent OnTurnOnContent;
	public GameEvent OnTurnOffContent;
	public string openSentence;

	private void Start() {
		animator = GetComponent<Animator>();
		textToSpeechAPI = GetComponent<TextToSpeechAPI>();
		audioSource = GetComponent<AudioSource>();
		StartCoroutine(CR_Speak(welcomeSentence));
	}
	

	IEnumerator CR_Speak(string content)
	{
		yield return textToSpeechAPI.CR_ConvertToAudioClip(content, userResponse.token);
		StartCoroutine(CR_SpeakRepeat(textToSpeechAPI.clip));
	}

	[ContextMenu("Speak")]
	IEnumerator CR_SpeakRepeat(AudioClip clip)
	{
		audioSource.clip = clip;
		while (true)
		{
			audioSource.Play();
			yield return new WaitForSeconds(repeatAfter);
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (!other.GetComponent<NetworkBehaviour>()) return;
		bool isLocalPlayer = other.GetComponent<NetworkBehaviour>().isLocalPlayer;
		// bool isLocalPlayer = true;
		bool isPlayerLayer = other.gameObject.layer == LayerMask.NameToLayer("Player");
		if (isPlayerLayer && isLocalPlayer)
		{
			TriggerWatching();
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<NetworkBehaviour>()) return;
		bool isLocalPlayer = other.GetComponent<NetworkBehaviour>().isLocalPlayer;
		// bool isLocalPlayer = true;
		bool isPlayerLayer = other.gameObject.layer == LayerMask.NameToLayer("Player");
		if (isPlayerLayer && isLocalPlayer)
		{
			TurnOffWatching();
		}

	}

	private void TurnOffWatching()
	{
		canvasHelper.gameObject.SetActive(false);
		animator.SetBool("Talk", false);
		audioSource.volume = 1;
		OnTurnOffContent.Raise();
	}

	private void TriggerWatching()
	{
		canvasHelper.gameObject.SetActive(true);
		animator.SetBool("Talk", true);
		audioSource.volume = 0;
		OnTurnOnContent.Raise();
	}
}
