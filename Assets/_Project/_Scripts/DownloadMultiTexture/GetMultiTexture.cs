using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.UI;

[Serializable]
public class FilesInfo
{
	public string path;
	public string name;
	public FilesInfo[] children;
}

public class GetMultiTexture : MonoBehaviour
{

	public EnvironmentVariablesContainer environmentVariablesContainer;
	public string fileRequest;
	public string token;
	public string hostName;
	public RectTransform content;
	public RawImage imagePrefab;
	public string fileType;
	// public int scaleRatio;
	float ratio;
	Texture2D currentTexture;
	FilesInfo result;

	public event System.Action<RawImage> OnAddImageEvent;
    // Start is called before the first frame update
    void OnEnable()
	{
		foreach(Transform child in content.transform)
        {
			GameObject.Destroy(child.gameObject);
        }
		ChooseRatio(fileType);
		GetRequest();
	}

	public void GetRequest()
	{
		StartCoroutine(CR_GetRequest(fileRequest));
	}

	void ChooseRatio(string type)
	{
		if (type.Equals("pdf"))
		{
			ratio = 1f/1.4142f;
		}
		if (type.Equals("docx"))
		{
			ratio = 1f/1.4142f;
		}
		if (type.Equals("pptx"))
		{
			ratio = 16f/9f;
		}
	}
	IEnumerator CR_GetRequest(string uri)
    {
		Debug.Log("Enter Function GetRequest");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
		{
			AddTokenHeader(webRequest);
			Debug.Log("Send Web Request");
			// Request and wait for the desired page.
			yield return webRequest.SendWebRequest();

			string[] pages = uri.Split('/');
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
					result = JsonUtility.FromJson<FilesInfo>(webRequest.downloadHandler.text);
					// LogFilesInfo(result);
					StartCoroutine(FillContent(result));
					// Debug.Log(result);
					break;
			}
		}
	}

	private void AddTokenHeader(UnityWebRequest webRequest)
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

	void LogFilesInfo(FilesInfo file)
	{
		Debug.Log(file.name);
		Debug.Log(file.path);
		Debug.Log(file.children);
		foreach (FilesInfo child in file.children)
		{
			Debug.Log(child.name);
			Debug.Log(child.path);
		}
	}

	void FixFilePath(FilesInfo file)
	{
		string[] splitPath;
		foreach (FilesInfo child in file.children)
		{
			splitPath = child.path.Split('/');
			child.path = String.Empty;
			for (int i = 0; i < 3; ++i)
			{
				child.path = splitPath[splitPath.Length - i - 1] + '/' + child.path;
			}
		}
	}

	IEnumerator FillContent(FilesInfo file)
	{
		// FixFilePath(file);
		foreach (FilesInfo child in file.children)
		{
			// Debug.Log(child.name);
			// string path = Path.Combine(webServerUrl, child.path);
			// string path = Combine(environmentVariablesContainer.environmentVariables.webServerUrl, child.path);
			string path = child.path;
			// Debug.Log("Download from " + path);
			yield return GetTexture(path);
			AddContent(currentTexture);
			// Debug.Log("Done with " + child.path);
		}

	}

	private void AddContent(Texture2D currentTexture)
	{
		RawImage rimage = Instantiate(imagePrefab, content);
		rimage.rectTransform.sizeDelta = new Vector2(content.rect.width, 0f);
		rimage.texture = currentTexture;
		rimage.GetComponent<AspectRatioFitter>().aspectRatio = ratio;
		if (OnAddImageEvent != null)
		{
			OnAddImageEvent(rimage);
		}
	}

	IEnumerator GetTexture(string url)
	{
		using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url,true))
        {
			AddTokenHeader(uwr);
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                currentTexture = DownloadHandlerTexture.GetContent(uwr);
				// currentTexture.Resize(currentTexture.width/10, currentTexture.height/10);
				// currentTexture.Apply();
				// TextureScale.Scale(currentTexture, currentTexture.width/scaleRatio, currentTexture.height/scaleRatio);
			}
        }

	}
	public static string Combine(string uri1, string uri2)
	{
		uri1 = uri1.TrimEnd('/');
		uri2 = uri2.TrimStart('/');
		uri2 = uri2.TrimEnd('/');
		return string.Format("{0}/{1}", uri1, uri2);
	}

}
