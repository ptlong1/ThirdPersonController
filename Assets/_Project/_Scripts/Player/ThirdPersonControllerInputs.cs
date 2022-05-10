using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class ThirdPersonControllerInputs : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    [Header("Movement Settings")]
    public bool analogMovement;
    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
        // Debug.Log(value.Get<Vector2>());
    }

    public void OnJump(InputValue value)
    {
        Debug.Log("Jump" + value.isPressed);
        JumpInput(value.isPressed);
    }
    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

	public void OnLockCursor(InputValue value)
	{
		if (value.isPressed)
		{
			cursorLocked = !cursorLocked;
			SetCursorState(cursorLocked);
		}
	}
	private void JumpInput(bool isPressed)
	{
        jump = isPressed;
	}

	private void SprintInput(bool isPressed)
	{
        sprint = isPressed;
	}

	private void LookInput(Vector2 vector2)
	{
        look = vector2;
	}

	private void MoveInput(Vector2 vector2)
	{
        move = vector2;
        // Debug.Log("Angle: " + Mathf.Atan2(vector2.x, vector2.y)*Mathf.Rad2Deg);
	}
    private void OnApplicationFocus(bool focusStatus) {
        // SetCursorState(focusStatus);
        // Debug.Log($"Application Focus {focusStatus}");
    }

    // private void OnApplicationPause(bool pauseStatus) {
    //     Debug.Log($"Application Paused {pauseStatus}");
    // }

	private void SetCursorState(bool focusStatus)
	{
		// if (EventSystem.current.IsPointerOverGameObject())
			Cursor.lockState = focusStatus?CursorLockMode.Locked:CursorLockMode.None;
	}

	// public void OnPointerDown(PointerEventData eventData)
	// {
    //     Debug.Log("pointer down");
    //     SetCursorState(true);
	// }
}
