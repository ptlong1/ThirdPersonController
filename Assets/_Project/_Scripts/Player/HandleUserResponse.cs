using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
public class HandleUserResponse : NetworkBehaviour
{
	public UserResponse userResponse;
    // Start is called before the first frame update

	public override void OnStartLocalPlayer(){
		GetComponent<RuntimeAvatarLoader>().isSyncAvatarUrl = true;
		GetComponent<RuntimeAvatarLoader>().CmdSetUserAvatar(userResponse.user.avatarUrl);
		GetComponent<PlayerName>().CmdSetPlayerName(userResponse.user.username);
		// GetComponent<RuntimeAvatarLoader>().userAvatar = (userResponse.user.url);
		// GetComponent<PlayerName>().playerName = (userResponse.user.username);
		
	}

	
}
