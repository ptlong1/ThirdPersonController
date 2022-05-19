using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class WebServerAPI  
{
	public static string resourceUrl = "http://localhost:8080/resources/all-resource-url";

	static string result;

	public static string Result { get => result; set => result = value; }

	private static void AddTokenHeader(UnityWebRequest webRequest, string token)
	{
		if (!String.IsNullOrWhiteSpace(token))
		{
			webRequest.SetRequestHeader("Authorization", "Bearer " + token);
		}
		else
		{
			Debug.Log("Token empty");
		}
	}

	public static IEnumerator CR_GetResouceJson(string token)
	{
        using (UnityWebRequest webRequest = UnityWebRequest.Get(resourceUrl))
		{
			AddTokenHeader(webRequest, token);
			Debug.Log("Send Web Request");
			// Request and wait for the desired page.
			yield return webRequest.SendWebRequest();

			string[] pages = resourceUrl.Split('/');
			int page = pages.Length - 1;

			switch (webRequest.result)
			{
				case UnityWebRequest.Result.ConnectionError:
				case UnityWebRequest.Result.DataProcessingError:
					Debug.LogError(pages[page] + ": Error: " + webRequest.error);
					break;
				case UnityWebRequest.Result.ProtocolError:
					Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
					break;
				case UnityWebRequest.Result.Success:
					Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
					Result = webRequest.downloadHandler.text;
					break;
			}
		}
	}
}
