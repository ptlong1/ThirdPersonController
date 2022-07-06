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
	public VoiceCanvas voiceCanvas;


	[ServerCallback]
	void Awake()
	{
	}
    public override void OnStartClient()
	{
		playerSpeaking.Callback += OnPlayerSpeakingUpdated;
		// Process initial SyncList payload
        for (int index = 0; index < playerSpeaking.Count; index++)
            OnPlayerSpeakingUpdated(SyncList<string>.Operation.OP_ADD, index, string.Empty, playerSpeaking[index]);

	}

	// private void OnDisable() {
	// 	if (NetworkClient.localPlayer == null) return;
	// 	Debug.Log("OnDisable");
	// 	string playerName = NetworkClient.localPlayer.GetComponent<PlayerName>().playerName;
	// 	Debug.Log(playerName);
	// 	Debug.Log("Contain name");
	// 	CmdRemovePlayerNameToList(playerName);
	// 	Debug.Log("End Disable");
	// }

	void OnPlayerSpeakingUpdated(SyncList<string>.Operation op, int index, string oldItem, string newItem)
    {
        switch (op)
        {
            case SyncList<string>.Operation.OP_ADD:
				voiceCanvas.Add(newItem);
                break;
            case SyncList<string>.Operation.OP_INSERT:
                // index is where it was inserted into the list
                // newItem is the new item
                break;
            case SyncList<string>.Operation.OP_REMOVEAT:
				voiceCanvas.Remove(oldItem);
                break;
            case SyncList<string>.Operation.OP_SET:
                // index is of the item that was changed
                // oldItem is the previous value for the item at the index
                // newItem is the new value for the item at the index
                break;
            case SyncList<string>.Operation.OP_CLEAR:
                // list got cleared
                break;
        }
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
	public void CmdRemovePlayerNameToList(string playerName)
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
