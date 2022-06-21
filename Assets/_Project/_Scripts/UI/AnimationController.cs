using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Mirror;
using UnityEngine.InputSystem;
public class AnimationController : MonoBehaviour
{
	public RectTransform selection;
	public bool isOn;
	Animator playerAnimator;
	public float duration;
	Coroutine selectionOffCR;
	public void SelectionOn()
	{
		isOn = true;
		selection
			.DOScale(Vector3.one, duration);
		if (selectionOffCR != null)
		{
			StopCoroutine(selectionOffCR);
		}
		selectionOffCR = StartCoroutine(CR_SelectionOff(4f));
	}

	IEnumerator CR_SelectionOff(float t)
	{
		yield return new WaitForSeconds(t);
		SelectionOff();
	}

	public void SelectionOff()
	{
		isOn = false;
		selection
			.DOScale(Vector3.zero, duration);
	}
	public void ToggleSelection()
	{
		if (isOn)
		{
			SelectionOff();
		}
		else
		{
			SelectionOn();
		}
	}
	private void Update() {
		if (Mouse.current.rightButton.wasPressedThisFrame)
		{
			Vector2 pos = (Mouse.current.position.ReadValue());
			selection.position = new Vector3(pos.x, pos.y, 0);
			// selection.gameObject.SetActive(true);
			SelectionOn();
		}
	}
	
	void Start()
	{
		SelectionOff();
	}

	public void PlayAnimation(string anim)
	{
		playerAnimator = NetworkClient.localPlayer.GetComponent<Animator>();
		playerAnimator.SetTrigger(anim);
		SelectionOff();
	}

}
