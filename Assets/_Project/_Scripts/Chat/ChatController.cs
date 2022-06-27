using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Assets._Project._Scripts.Chat
{
	public class ChatController : NetworkBehaviour
	{
		public ChatUI chatUI;
        public void OnEndEdit(string input)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetButtonDown("Submit"))
                SendText(input);
        }
		public void SendText()
		{
			if (!string.IsNullOrWhiteSpace(chatUI.inputField.text))
			{
				// CmdSendText(chatUI.inputField.text);
				SendText(chatUI.inputField.text);
				chatUI.ClearAndFocusInput();
			}
		}
		public void SendText(string message)
		{
			if (!string.IsNullOrWhiteSpace(message))
			{
				// Debug.Log("ABCYASDAS");
				// Debug.Log(NetworkClient.localPlayer);
				NetworkClient.localPlayer.GetComponent<NetworkAnimator>().SetTrigger("TalkTrigger");
				NetworkClient.localPlayer.GetComponent<PlayerSoundNetwork>().CmdPlayTalkSound();
				CmdSendText(message);
				chatUI.ClearAndFocusInput();
			}
		}		
		
		[Command(requiresAuthority = false)]
		public void CmdSendText(string msg, NetworkConnectionToClient sender = null)
		{
			if (!string.IsNullOrWhiteSpace(msg))
			{
				string playerName = sender.identity.GetComponent<PlayerName>().playerName;
				string roomName = sender.identity.GetComponent<PlayerController>().roomName;
				RpcReceiveText(playerName, roomName, msg);
				BubbleController bubbleController =	sender.identity.GetComponent<BubbleController>();
				bubbleController.RpcTurnOn(msg);
			}
		}

		[ClientRpc]
		public void RpcReceiveText(string playerName, string roomName, string msg)
		{
			if (NetworkClient.localPlayer.GetComponent<PlayerController>().roomName != roomName) return;
			string text = $"[{playerName}]: {msg}";
			chatUI.AppendMessage(text);
		}
	}
}