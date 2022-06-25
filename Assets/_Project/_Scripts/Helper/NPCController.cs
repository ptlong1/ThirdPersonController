using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NPCController : MonoBehaviour
{
	public HelperConversation canvasHelper;
	Animator animator;

	private void Start() {
		animator = GetComponent<Animator>();
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
	}

	private void TriggerWatching()
	{
		canvasHelper.gameObject.SetActive(true);
		animator.SetBool("Talk", true);
	}
}
