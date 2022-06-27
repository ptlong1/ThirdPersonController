
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSoundNetwork : NetworkBehaviour 
{
	public PlayerSound playerSound;
    void Start()
    {
		playerSound = GetComponentInChildren<PlayerSound>();
    }


	[Command]
	public void CmdPlayTalkSound()
	{
		Debug.Log("CMDPlayTalkSound");
		RpcPlayTalkSound();
	}

	[ClientRpc]
	public void RpcPlayTalkSound()
	{
		Debug.Log("RpcPlayTalkSound");
		playerSound.PlayTalkSound();
	}
}
