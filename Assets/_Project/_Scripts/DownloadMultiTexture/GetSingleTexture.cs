using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetSingleTexture : MonoBehaviour
{
	public string fileRequest;
	public RawImage image;
	Texture2D currentTexture;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(CR_AppendTexture());
    }

	IEnumerator CR_AppendTexture()
	{
		yield return CR_GetTexture(fileRequest);
		image.texture = currentTexture;
		image.GetComponent<AspectRatioFitter>().aspectRatio = (1f*currentTexture.width)/currentTexture.height;
	}

	IEnumerator CR_GetTexture(string url)
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
}
