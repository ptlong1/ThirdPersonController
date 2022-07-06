using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class VoiceToast : MonoBehaviour
{
	public TMP_Text text;

	public void SetText(string txt)
	{
		text.text = txt;
	}

	internal void Remove()
	{
		Destroy(gameObject);
	}
}
