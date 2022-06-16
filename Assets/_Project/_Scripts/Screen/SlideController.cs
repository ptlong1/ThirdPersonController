using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using Cinemachine;

public class SlideController :NetworkBehaviour 
{
	public string controllerName;
	public UserResponse userResponse;
	public SlideView slideView;
	public GameObject controllerUI;
	public Button watchSlideBtn;
	[SyncVar(hook = nameof(SetIndex))]
	public int currentIndex;
	Camera camSlide;
	CinemachineFreeLook freeLookCamera;
	Camera mainCamera;

	private void Start() {
		watchSlideBtn.gameObject.SetActive(true);
		camSlide = GameObject.FindGameObjectWithTag("CameraSlide").GetComponent<Camera>();
		camSlide.enabled = true;
		camSlide.gameObject.SetActive(false);
		freeLookCamera = FindObjectOfType<CinemachineFreeLook>();
		mainCamera = Camera.main;
	}

	public void ChangeToCameraSlide()
	{
		bool vl = camSlide.gameObject.activeSelf;
		mainCamera.gameObject.SetActive(vl);
		camSlide.gameObject.SetActive(!vl);	
	}

	public void SetUp(SlideView slide) {
		slideView = slide;
		slideView.CurrentIndex = currentIndex;
		controllerUI.SetActive(CheckIfOwner());
	}

	void SetIndex(int oldIdx, int newIdx)
	{
		if (slideView == null) return;
		slideView.CurrentIndex = currentIndex;
		currentIndex = slideView.CurrentIndex;
	}	

	bool CheckIfOwner()
	{
		return slideView.hostName == userResponse.user.username;
	}
	public void AddToIdx(int value)
	{
		slideView.AddToIdx(value);
	}

	[Command(requiresAuthority = false)]
	public void CmdAddToIdx(int value)
	{
		// RpcAddToIdx(value);
		currentIndex += value;
	}

	[ClientRpc]
	public void RpcAddToIdx(int value)
	{
		slideView.AddToIdx(value);
	}
}
