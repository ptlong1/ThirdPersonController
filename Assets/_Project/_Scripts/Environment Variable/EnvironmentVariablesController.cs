using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EnvironmentVariablesController : MonoBehaviour
{
	public EnvironmentVariablesContainer environmentVariablesContainer;
	public string resultJson;
	IEnumerator CR_GetEnvironmentJson(string url)
	{
		string newUrl = url;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(newUrl))
		{
			Debug.Log("Send Web Request");
			// Request and wait for the desired page.
			yield return webRequest.SendWebRequest();

			string[] pages = url.Split('/');
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
					resultJson = webRequest.downloadHandler.text;
					break;
			}
		}
	}
	public IEnumerator CR_ParseEnvironmentJson()
	{
		yield return CR_GetEnvironmentJson(environmentVariablesContainer.EnvironmentUrl);
		// yield return null;
		environmentVariablesContainer.Parse(resultJson);
	}
}
