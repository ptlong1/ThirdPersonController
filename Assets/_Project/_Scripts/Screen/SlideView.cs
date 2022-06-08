using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideView : MonoBehaviour
{
	GetMultiTexture getMultiTexture;
	List<RawImage> listSlide;
	int currentIndex;
	public SlideController slideController;

	public int CurrentIndex { 
		get => currentIndex; 
		set {
			if (listSlide.Count == 0)
			{
				currentIndex = value;
				return;
			}
			Turn(currentIndex, false);
			int temp = value;
			if (temp < 0)
				temp += listSlide.Count;
			currentIndex = temp % listSlide.Count; 
			Turn(currentIndex, true);
		}
	}

	private void Turn(int currentIndex, bool vl)
	{
		if (currentIndex >= listSlide.Count) 
		{
			Debug.Log("Current Index Out of list Slide count");
			return;
		}
		listSlide[currentIndex].gameObject.SetActive(vl);
	}
	public void AddToIdx(int value)
	{
		CurrentIndex += value;
	}

	void Awake()
	{
		getMultiTexture = GetComponentInParent<GetMultiTexture>();
		getMultiTexture.OnAddImageEvent += AddChildSlide;
		listSlide = new List<RawImage>();
		currentIndex = 0;
	}

	private void OnEnable() {
		StartCoroutine(CR_FindSlideController());
	}
	IEnumerator CR_FindSlideController()
	{
		while (slideController == null)
		{
			slideController = FindObjectOfType<SlideController>();
			yield return null;
		}
		Debug.Log("Find Slide Controller");
		slideController.SetUp(this);
	}

	void AddChildSlide(RawImage rawImage)
	{
		listSlide.Add(rawImage);
		if (listSlide.Count > 1)
			rawImage.gameObject.SetActive(false);
		CurrentIndex = currentIndex;
	}
}
