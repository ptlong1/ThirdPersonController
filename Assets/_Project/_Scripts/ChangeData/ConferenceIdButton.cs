using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConferenceIdButton : MonoBehaviour
{
	private string id;
	private int idx;
	public event System.Action TransferToId;
	public TMP_Text text;

	public string Id { 
		get {
			return id;
		}
		set {
			id = value;
			text.text = id;
		}
	}

	public int Idx { get => idx; set => idx = value; }

	public void OnClickButton()
	{
		// Debug.Log("Click Button");
		if (TransferToId != null)
		{
			TransferToId();
			// Debug.Log("Do action");
		}
		else
		{
			// Debug.Log($"{Id} button not action null");
		}
	}
}
