using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Video;


public enum screenTypeEnum {
	Picture,
	Document,
	Video
}
[RequireComponent(typeof(ConferenceData))]
public class Screen : MonoBehaviour
{
	public screenTypeEnum screenType;
	public GameObject picturePrefab;
	public GameObject documentPrefab;
	public GameObject videoPrefab;
	public string urlContent;
	public GameObject content;
	public GameObject confirmUI;
    // Start is called before the first frame update
    void Start()
    {
        content.SetActive(false);
		confirmUI.SetActive(false);
		picturePrefab.SetActive(false);
		documentPrefab.SetActive(false);
		videoPrefab.SetActive(false);
    }

	void TurnOnContent(bool value)
	{
		ReadyContent();
		if 	(screenType == screenTypeEnum.Picture)
		{
			picturePrefab.SetActive(value);
		}
		if (screenType == screenTypeEnum.Document)
		{
			documentPrefab.SetActive(value);
		}
		if (screenType == screenTypeEnum.Video)
		{
			videoPrefab.SetActive(value);
		}
	}

	void ReadyContent()
	{
		picturePrefab.GetComponent<GetSingleTexture>().fileRequest = urlContent;
		documentPrefab.GetComponent<GetMultiTexture>().fileRequest = urlContent;
		videoPrefab.GetComponent<VideoPlayer>().url = urlContent;
	}

	// [ClientCallback]
	void OnTriggerEnter(Collider other) {
		// Debug.Log("Trigger Enter");
		if (!other.GetComponent<NetworkBehaviour>()) return;
		bool isLocalPlayer = other.GetComponent<NetworkBehaviour>().isLocalPlayer;
		// bool isLocalPlayer = true;
		bool isPlayerLayer =other.gameObject.layer == LayerMask.NameToLayer("Player"); 
		if (isPlayerLayer && isLocalPlayer)
		{
			TurnOnConfirmUI(true);
			// ReadyContent();
			TurnOnContent(true);
		}
	}

	// [ClientCallback]
	private void OnTriggerExit(Collider other) {
		if (!other.GetComponent<NetworkBehaviour>()) return;
		bool isLocalPlayer = other.GetComponent<NetworkBehaviour>().isLocalPlayer;
		// bool isLocalPlayer = true;
		bool isPlayerLayer =other.gameObject.layer == LayerMask.NameToLayer("Player"); 
		if (isPlayerLayer && isLocalPlayer)
		{
			TurnOnConfirmUI(false);
			TurnOnContent(false);
		}
		
	}

	private void TurnOnConfirmUI(bool value)
	{
		confirmUI.gameObject.SetActive(value);
	}
}
