using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Mirror;
using UnityEngine.InputSystem;
using TMPro;
public class BubbleController : NetworkBehaviour 
{
	public RectTransform bubbleNear;
	public TMP_Text textNear; 
	public RectTransform bubbleFar;
	public float threshhold;
	Coroutine cr_TurnOff;

	void TurnOn()
	{
		// if == networkclient.localplayer return
		// localplayer.transform.distance > threshhold => far
		//else near
		// if (GetComponent<NetworkIdentity>().isLocalPlayer) return;
		Transform localPlayer = NetworkClient.localPlayer.transform;
		if (Vector3.Distance(transform.position, localPlayer.position) <= threshhold)
		{
			bubbleNear.gameObject.SetActive(true);
			bubbleFar.gameObject.SetActive(false);
		}
		else
		{
			bubbleNear.gameObject.SetActive(false);
			bubbleFar.gameObject.SetActive(true);
		}
		if (cr_TurnOff != null)
		{
			StopCoroutine(cr_TurnOff);
		}
		cr_TurnOff =  StartCoroutine(CR_TurnOff(3f));
	}
	IEnumerator CR_TurnOff(float t)
	{
		yield return new WaitForSeconds(t);
		TurnOff();
	}

	void TurnOff()
	{
		bubbleNear.gameObject.SetActive(false);
		bubbleFar.gameObject.SetActive(false);
	}

	public void SetText(string chatText)
	{
		textNear.text = chatText;
	}

	[Command]
	void CmdTurnOn()
	{
		// RpcTurnOn();
	}

	[ClientRpc]
	public void RpcTurnOn(string msg)
	{
		SetText(msg);
		TurnOn();
	}

}
