using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using DG.Tweening;
using UnityEngine.UI;
public class VoiceToast : MonoBehaviour
{
	public TMP_Text text;

	public void SetText(string txt)
	{
		text.text = txt;
	}

	internal void Remove()
	{
		RectTransform rect = GetComponent<RectTransform>();
		Image image = GetComponent<Image>();
		rect.DOScaleY(0f, 0.5f);
		image.DOFade(0f, 0.5f).OnComplete(() => Destroy(gameObject));
	}
}
