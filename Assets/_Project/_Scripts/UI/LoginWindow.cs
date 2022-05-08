using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Mirror;
public class LoginWindow : MonoBehaviour
{
	public TMP_InputField username;
	public TMP_InputField password;
	public LoginAPI loginAPI;
	public Button loginButton;
	NetworkManager manager;
    // Start is called before the first frame update

	void Start(){
		manager = FindObjectOfType<NetworkManager>();
	}
	public void UpdateInfo()
	{
		loginAPI.username = username.text;
		loginAPI.password = password.text;
	}

	public void OnClickButton()
	{
		StartCoroutine(TryLogin());
	}
	public IEnumerator TryLogin()
	{
		loginButton.interactable = false;
		yield return StartCoroutine(loginAPI.Login(loginSuccess, loginFailure));
		loginButton.interactable = true;
	}

	private void loginFailure()
	{
		Debug.Log("Login Failure");
		//show model windows about fail
	}

	private void loginSuccess()
	{
		Debug.Log("Login Sucess");
		gameObject.SetActive(false);
		manager.StartClient();
	}
}
