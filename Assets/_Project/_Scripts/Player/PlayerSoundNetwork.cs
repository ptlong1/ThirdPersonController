
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSoundNetwork : NetworkBehaviour 
{
	PlayerSound playerSound;
    void Start()
    {
		playerSound = GetComponentInChildren<PlayerSound>();
    }


	[Command]
	public void CmdPlayTalkSound()
	{
		RpcPlayTalkSound();
	}

	[ClientRpc]
	public void RpcPlayTalkSound()
	{
		playerSound.PlayTalkSound();
	}
}
