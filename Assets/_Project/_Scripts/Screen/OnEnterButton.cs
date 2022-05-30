using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class OnEnterButton : MonoBehaviour
{
	public UnityEvent OnEnterEvent;

	void Update() {
		if (Keyboard.current.enterKey.wasPressedThisFrame)
		{
			if (OnEnterEvent != null)
			{
				OnEnterEvent.Invoke();
			}
		}
	}
}
