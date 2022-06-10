using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class TestDeltaTouchScreen : MonoBehaviour
{
	public TMP_Text text;
	
	void Start(){
		// TouchSimulation.Enable();
	}

	public void PrintDelta(InputAction.CallbackContext context)
	{
		Debug.Log("Print Delta");
		text.text = context.ReadValue<Vector2>().ToString();
	}
}
