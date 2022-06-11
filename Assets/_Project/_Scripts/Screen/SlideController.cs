using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SlideController :NetworkBehaviour 
{
	public string controllerName;
	public UserResponse userResponse;
	public SlideView slideView;
	public GameObject controllerUI;
	[SyncVar(hook = nameof(SetIndex))]
	public int currentIndex;

	private void Start() {
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
