using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Video;
using ScriptableObjectArchitecture;

namespace Assets._Project._Scripts.Screen
{
	public enum screenTypeEnum
	{
		Picture,
		Document,
		Video
	}
	[RequireComponent(typeof(ConferenceObjectData))]
	public class ConferenceScreen : MonoBehaviour
	{
		public screenTypeEnum screenType;
		public GameObject picturePrefab;
		public GameObject documentPrefab;
		public GameObject videoPrefab;
		public string urlContent;
		public string token;
		public GameObject content;
		public GameObject confirmUI;
		public GameEvent OnTurnOnContent;
		public GameEvent OnTurnOffContent;
		public bool defaultOn;
		// Start is called before the first frame update
		void Start()
		{
			// content.SetActive(false);
			confirmUI.SetActive(false);
			// picturePrefab.SetActive(false);
			// documentPrefab.SetActive(false);
			// videoPrefab.SetActive(false);
		}

		public void TurnOnContent(bool value)
		{
			ReadyContent();
			Debug.Log("In TurnOnContent " + value);
			if (screenType == screenTypeEnum.Picture)
			{
			Debug.Log("In TurnOnContent Pictrue " + value);
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
			if (value)
			{
				if (OnTurnOnContent != null)
				OnTurnOnContent.Raise();
			}
			else{
				if (OnTurnOffContent != null)
				OnTurnOffContent.Raise();
			}
		}

		public void ShowContent()
		{
			Debug.Log("Show Content Banner");
			gameObject.SetActive(true);
			TurnOnContent(true);
		}

		void ReadyContent()
		{
			picturePrefab.GetComponent<GetSingleTexture>().fileRequest = urlContent;
			picturePrefab.GetComponent<GetSingleTexture>().token = token;

			documentPrefab.GetComponent<GetMultiTexture>().fileRequest = urlContent;
			documentPrefab.GetComponent<GetMultiTexture>().token = token;

			videoPrefab.GetComponent<VideoPlayer>().url = urlContent + "&token=" + token;
			// videoPrefab.GetComponent<VideoPlayer>().url = urlContent;
		}

		[ContextMenu("Restart Content")]
		public void RestartContent()
		{
			ReadyContent();
			if (content.activeInHierarchy)
			{
				content.SetActive(false);
				content.SetActive(true);

			}
		}

		// [ClientCallback]
		void OnTriggerEnter(Collider other)
		{
			// Debug.Log("Trigger Enter");
			if (!other.GetComponent<NetworkBehaviour>()) return;
			bool isLocalPlayer = other.GetComponent<NetworkBehaviour>().isLocalPlayer;
			// bool isLocalPlayer = true;
			bool isPlayerLayer = other.gameObject.layer == LayerMask.NameToLayer("Player");
			if (isPlayerLayer && isLocalPlayer)
			{
				TurnOnConfirmUI(true);
				// ReadyContent();
				TurnOnContent(true);
			}
		}

		// [ClientCallback]
		private void OnTriggerExit(Collider other)
		{
			if (!other.GetComponent<NetworkBehaviour>()) return;
			bool isLocalPlayer = other.GetComponent<NetworkBehaviour>().isLocalPlayer;
			// bool isLocalPlayer = true;
			bool isPlayerLayer = other.gameObject.layer == LayerMask.NameToLayer("Player");
			if (isPlayerLayer && isLocalPlayer)
			{
				TurnOnConfirmUI(false);
				TurnOnContent(false);
				content.SetActive(false);
			}

		}

		private void TurnOnConfirmUI(bool value)
		{
			confirmUI.gameObject.SetActive(value);
		}
	}
}