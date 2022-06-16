using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class ConferenceMetaData
{
	public string _id;
	public string name;
	public string startTime;
	public string endTime;
}

public class ConferenceMetaDataResponse
{
	public ConferenceMetaData[] conferenceMetaDatas;
}
public static class WebServerAPI  
{
	// public static string resourceUrl = "http://localhost:8080/resources/";
	// public static string conferenceUrl = "http://localhost:8080/conference";

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
	public static string Combine(string uri1, string uri2)
	{
		uri1 = uri1.TrimEnd('/');
		uri2 = uri2.TrimStart('/');
		uri2 = uri2.TrimEnd('/');
		return string.Format("{0}/{1}", uri1, uri2);
	}

	public static IEnumerator CR_GetResouceJson(string token, string conferenceUrl, string conferenceId)
	{
		string newUrl = Combine(conferenceUrl, conferenceId);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(newUrl))
		{
			AddTokenHeader(webRequest, token);
			Debug.Log("Send Web Request");
			// Request and wait for the desired page.
			yield return webRequest.SendWebRequest();

			string[] pages = newUrl.Split('/');
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
	public static IEnumerator CR_GetConferenceInfo(string conferenceUrl)
	{
		string newUrl = conferenceUrl;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(newUrl))
		{
			// AddTokenHeader(webRequest, token);
			// Debug.Log("Send Web Request");
			// Request and wait for the desired page.
			yield return webRequest.SendWebRequest();

			string[] pages = newUrl.Split('/');
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
