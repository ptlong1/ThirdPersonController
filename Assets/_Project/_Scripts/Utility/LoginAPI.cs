using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoginAPI : MonoBehaviour
{
	public EnvironmentVariablesContainer environmentVariablesContainer;
	public string username;
	public string password;
	public UserResponse userResponse;
	public string resultJson;

	public void StartLogin()
	{
		StartCoroutine(Login(username, password));
	}

	public IEnumerator Login(string usr, string pw)
	{
		string postBody = $"{{\"username\": \"{usr}\",\"password\": \"{pw}\"}}";
		Debug.Log(postBody);
		using (var req = new UnityWebRequest(environmentVariablesContainer.environmentVariables.urlLogin, "POST"))
		{
			byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(postBody);
			req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
			req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			req.SetRequestHeader("Content-Type", "application/json");

			yield return req.SendWebRequest();

			if (req.result != UnityWebRequest.Result.Success)
			{
				Debug.Log("Error While Sending: " + req.error);
				Debug.Log("Received: " + req.downloadHandler.text);
			}
			else
			{
				Debug.Log("Received: " + req.downloadHandler.text);
			}
		}
	}

	public IEnumerator Login( System.Action successCB, System.Action failureCB)
	{
		string usr = username;
		string pw = password;
		string postBody = $"{{\"username\": \"{usr}\",\"password\": \"{pw}\"}}";
		// Debug.Log(postBody);
		using (var req = new UnityWebRequest(environmentVariablesContainer.environmentVariables.urlLogin, "POST"))
		{
			byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(postBody);
			req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
			req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			req.SetRequestHeader("Content-Type", "application/json");

			yield return req.SendWebRequest();

			if (req.result != UnityWebRequest.Result.Success)
			{
				Debug.Log("Error While Sending: " + req.error);
				Debug.Log("Received: " + req.downloadHandler.text);
				failureCB();
			}
			else
			{
				Debug.Log("Received: " + req.downloadHandler.text);
				userResponse.ParseJson(req.downloadHandler.text);
				successCB();
			}
		}
	}
	public IEnumerator CR_GetAllConferenceMetaData()
	{
		yield return WebServerAPI.CR_GetConferenceInfo(environmentVariablesContainer.environmentVariables.conferenceUrl);
		resultJson = WebServerAPI.Result;
	}
}
