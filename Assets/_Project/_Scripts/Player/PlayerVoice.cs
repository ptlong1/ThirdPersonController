using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerVoice : NetworkBehaviour
{
	VoiceDetection voiceDetection;
	void Start()
	{
		{
			VoiceDetection[] voiceDetections = FindObjectsOfType<VoiceDetection>();
			Debug.Log(gameObject.scene.name);
			Debug.Log(voiceDetections.Length);
			foreach(VoiceDetection voice in voiceDetections)
			{
				Debug.Log(voice.gameObject.scene.name);
				if (voice.gameObject.scene.name.Equals(gameObject.scene.name))
				
				{
					
					voiceDetection = voice;
					
					break;
				}
			}

		}
	}
	[ServerCallback]
	private void OnDisable() {
		Debug.Log("PlayerVoice Disable");
		voiceDetection.playerSpeaking.Remove(GetComponent<PlayerName>().playerName);
	}
}
