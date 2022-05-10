using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
public class PlayerName : NetworkBehaviour
{
	[SyncVar(hook =nameof(SetName))]
	public string playerName;

	public TMP_Text nameDisplay;

	void SetName(string oldName, string newName)
	{
		nameDisplay.text = newName;
	}
	[Command]
	public void CmdSetPlayerName(string newName)
	{
		playerName = newName;
	}
}
