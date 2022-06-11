using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Video;
using ScriptableObjectArchitecture;
using Assets._Project._Scripts.DynamicData;

namespace Assets._Project._Scripts.Screen
{
	public enum screenTypeEnum
	{
		Picture,
		Document,
		Video,
		Slide
	}
	[RequireComponent(typeof(ConferenceObjectData))]
	public class ConferenceScreen : MonoBehaviour
	{
		public screenTypeEnum screenType;
		public GameObject picturePrefab;
		public GameObject documentPrefab;
		public GameObject videoPrefab;
		public GameObject slidePrefab;
		public string urlContent;
		public string token;
		public string hostName;
		public GameObject content;
		public GameObject confirmUI;
		public GameEvent OnTurnOnContent;
		public GameEvent OnTurnOffContent;
		public bool defaultOn;
		public bool useWorldSpace;
		// Start is called before the first frame update
		void Start()
		{
			// content.SetActive(false);
			confirmUI.SetActive(false);
		}

		void TurnOffAll()
		{
			picturePrefab.SetActive(false);
			documentPrefab.SetActive(false);
			videoPrefab.SetActive(false);
			slidePrefab.SetActive(false);
		}

		public void TurnOnContent(bool value)
		{
			TurnOffAll();
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
			if (screenType == screenTypeEnum.Slide)
			{
				slidePrefab.SetActive(value);
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

			slidePrefab.GetComponent<GetMultiTexture>().fileRequest = urlContent;
			slidePrefab.GetComponent<GetMultiTexture>().token = token;
			slidePrefab.GetComponent<GetMultiTexture>().hostName = hostName;

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
				TriggerWatching();
			}
		}

		public void TriggerWatching()
		{
			if (useWorldSpace) return;
			// TurnOnConfirmUI(true);
			// ReadyContent();
			TurnOnContent(true);
			content.SetActive(true);
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
				TurnOffWatching();
			}

		}

		public void TurnOffWatching()
		{
			if (useWorldSpace) return;
			TurnOnConfirmUI(false);
			TurnOnContent(false);
			content.SetActive(false);
		}

		private void TurnOnConfirmUI(bool value)
		{
			confirmUI.gameObject.SetActive(value);
		}
	}
}