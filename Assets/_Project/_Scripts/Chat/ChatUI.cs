using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System.Collections;

namespace Assets._Project._Scripts.Chat
{

	public class ChatUI :MonoBehaviour 
	{
		public TMP_InputField inputField;
		public TMP_Text chatHistory;
		public Scrollbar scrollbar;

		public void ClearAndFocusInput()
		{
			inputField.text = string.Empty;
			inputField.ActivateInputField();
		}

		public void AppendMessage(string message)
		{
			StartCoroutine(AppendAndScroll(message));
		}

		IEnumerator AppendAndScroll(string message)
		{
			chatHistory.text += message + "\n";

			yield return null;
			yield return null;

			scrollbar.value = 0;
		}

	}
}