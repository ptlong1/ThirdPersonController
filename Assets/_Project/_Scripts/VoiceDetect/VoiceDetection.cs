using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using EasyUI.Toast;
public class VoiceDetection : NetworkBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern bool isSpeaking();
#endif

	public readonly SyncList<string> playerSpeaking = new SyncList<string>();
	bool oldState;
	public bool editorValue;


	[ServerCallback]
	void Awake()
	{
	}

	[Command]
	void CmdSyncList()
	{

	}

	public bool IsLocalPlayerSpeaking()
	{
		#if UNITY_EDITOR
    		return editorValue; // value to return in Play Mode (in the editor)
		#elif UNITY_WEBGL
    		return isSpeaking(); // value based on the current browser
		#else
    		return false; // value for builds other than WebGL
		#endif
	}

	[ClientCallback]
	private void Update() {
		if (oldState == false && IsLocalPlayerSpeaking())
		{
			Debug.Log("Add");
			CmdAddPlayerNameToList(NetworkClient.localPlayer.GetComponent<PlayerName>().playerName);
			oldState = true;
		}
		if (oldState == true && !IsLocalPlayerSpeaking()) 
		{
			//remove list
			Debug.Log("Remove");
			CmdRemovePlayerNameToList(NetworkClient.localPlayer.GetComponent<PlayerName>().playerName);
			oldState = false;
		}
	}

	[Command(requiresAuthority = false)]
	void CmdAddPlayerNameToList(string playerName)
	{
		playerSpeaking.Add(playerName);
	}
	
	// [Command(requiresAuthority = false)]
	// void CmdAddPlayerNameToList(PlayerName playerName)
	// {
	// 	RpcAddPlayerName(playerName);
	// }

	[ClientRpc]
	void RpcAddPlayerName(PlayerName playerName)
	{
		Debug.Log("RPC ADD");
		// playerSpeaking.Add(playerName);
	}

	[Command(requiresAuthority = false)]
	void CmdRemovePlayerNameToList(string playerName)
	{
		playerSpeaking.Remove(playerName);
		// RpcRemovePlayerName(playerName);
	}

	[ClientRpc]
	void RpcRemovePlayerName(PlayerName playerName)
	{
		// playerSpeaking.Remove(playerName);
	}

	void UpdateUI()
	{
		Toast.Show("There are " + playerSpeaking.Count.ToString() + " people talking", 1f, 
			ToastColor.Blue, 
			ToastPosition.TopRight);
	}
}
