using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Mirror;
using ScriptableObjectArchitecture;
using EasyUI.Toast ;
using Assets._Project._Scripts.DynamicData;

public class LoginWindow : MonoBehaviour
{
	public TMP_Dropdown conferenceNameDropdown;
	public TMP_InputField username;
	public TMP_InputField password;
	public LoginAPI loginAPI;
	public Button loginButton;
	NetworkManager manager;
	public ConferenceData conferenceData;
	public GameEvent OnLoginSuccess;
	ConferenceMetaDataResponse conferenceMetaDataResponse;
    // Start is called before the first frame update

	void Start(){
		manager = FindObjectOfType<NetworkManager>();
		StartCoroutine(CR_GetAllConferenceMetaData());
	}

	IEnumerator CR_GetAllConferenceMetaData()
	{
		yield return WebServerAPI.CR_GetConferenceInfo();
		string result = WebServerAPI.Result;
		string fixJson = "{\"conferenceMetaDatas\":" + result + "}";
		conferenceMetaDataResponse = JsonUtility.FromJson<ConferenceMetaDataResponse>(fixJson);
		SetupConferenceName();
	}

	void SetupConferenceName()
	{
		for(int i = 0; i < conferenceMetaDataResponse.conferenceMetaDatas.Length; ++i)
		{
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(
				conferenceMetaDataResponse.conferenceMetaDatas[i].name
			);
			conferenceNameDropdown.options.Add(optionData);
		}
		conferenceNameDropdown.RefreshShownValue();
	}

	public void UpdateConferenceId(int idx)
	{
		conferenceData.ConferenceId = conferenceMetaDataResponse.conferenceMetaDatas[idx]._id;
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
		Toast.Show("Login Fail", 3f, ToastColor.Red, ToastPosition.BottomCenter);
		//show model windows about fail
	}

	private void loginSuccess()
	{
		Debug.Log("Login Sucess");
		Toast.Show("Login Success", 3f, ToastColor.Blue, ToastPosition.BottomCenter);
		gameObject.SetActive(false);
		OnLoginSuccess.Raise();
		// manager.StartClient();
	}
}
