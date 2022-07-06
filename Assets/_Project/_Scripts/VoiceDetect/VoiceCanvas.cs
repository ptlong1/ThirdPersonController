using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceCanvas : MonoBehaviour
{
	public VoiceToast voiceToast;
	public Transform context;
	public Dictionary<string, VoiceToast> currentToast;
	private void Awake() {
		currentToast = new Dictionary<string, VoiceToast>();
	}

	public void Add(string playerName)
	{
		VoiceToast newToast = Instantiate(voiceToast, context);
		newToast.SetText(playerName);
		currentToast.Add(playerName, newToast);
	}

	public void Remove(string playerName)
	{
		VoiceToast toast = currentToast[playerName];
		toast.Remove();
		currentToast.Remove(playerName);
	}
}
