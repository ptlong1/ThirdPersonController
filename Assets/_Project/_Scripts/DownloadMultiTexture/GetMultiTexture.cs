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

	public string webServerUrl;
	public string fileRequest;
	public RectTransform content;
	public RawImage imagePrefab;
	public string fileType;
	public int scaleRatio;
	float ratio;
	Texture2D currentTexture;
	FilesInfo result;
    // Start is called before the first frame update
    void OnEnable()
	{
		// Debug.Log("ABC");
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
			ratio = 4f/3f;
		}
	}
	IEnumerator CR_GetRequest(string uri)
    {
		Debug.Log("Enter Function GetRequest");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
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

	IEnumerator FillContent(FilesInfo file)
	{
		foreach (FilesInfo child in file.children)
		{
			// Debug.Log(child.name);
			// string path = Path.Combine(webServerUrl, child.path);
			string path = Combine(webServerUrl, child.path);
			Debug.Log("Download from " + path);
			yield return GetTexture(path);
			AddContent(currentTexture);
			Debug.Log("Done with " + child.path);
		}

	}

	private void AddContent(Texture2D currentTexture)
	{
		RawImage rimage = Instantiate(imagePrefab, content);
		rimage.texture = currentTexture;
		rimage.GetComponent<AspectRatioFitter>().aspectRatio = ratio;
	}

	IEnumerator GetTexture(string url)
	{
		using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
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
		return string.Format("{0}/{1}", uri1, uri2);
	}

}
