using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Web;
using DG.Tweening;
using UnityEngine.Networking;

public class UpdateDataWindow : MonoBehaviour
{
	public TMP_Text headerId;
	private string id;
	public string Id
	{
		get {return id;}
		set { 
			id = value;
			headerId.text = id;
		}	
	}
	private string url;
	public string Url 
	{
		get {return url;}
		set { url = value;}
	}

	string simpleUrl;


	private string type;
	public string Type { get => type; set => type = value; }
	private string token;
	public string Token { get => token; set => token = value; }
	public string SimpleUrl { get => simpleUrl; set => simpleUrl = value; }

	public GameObject updateTable;
	public string updateUrlApi;

	public UserResponse userResponse;

	// Start is called before the first frame update
	void Start()
    {
        
    }

	[ContextMenu("Parse Url")]
	public void ParseUrl()
	{
		var uri = new Uri(Url);
		var query = HttpUtility.ParseQueryString(uri.Query);
		// Debug.Log(query.Get("token"));
		// Debug.Log(query.Get("type"));
		Token = query.Get("token");
		Type = query.Get("type");
		SimpleUrl = uri.GetLeftPart(UriPartial.Path);
	}

	public void OpenUI()
	{
		gameObject.SetActive(true);
		updateTable.transform.DOScale(1f, 0.3f).From(0f);
	}
	public void CloseUI()
	{
		updateTable.transform
			.DOScale(0f, 0.3f)
			.From(1f)
			.OnComplete( () => gameObject.SetActive(false));
	}

	private void AddTokenHeader(UnityWebRequest webRequest)
	{
		if (!String.IsNullOrWhiteSpace(Token))
		{
			webRequest.SetRequestHeader("Authorization", "Bearer " + Token);
		}
		else
		{
			Debug.Log("Token empty");
		}
	}

	public void UpdateData()
	{
		ParseUrl();
		if (String.IsNullOrWhiteSpace(Token) 
			|| String.IsNullOrWhiteSpace(Type) 
			|| String.IsNullOrWhiteSpace(SimpleUrl))
			{
				Debug.Log("Data not valid");
				return;
			}
		StartCoroutine(CR_UpdateData());
	}
	IEnumerator CR_UpdateData()
	{
		string postBody = $"{{\"id\": \"{Id}\",\"url\": \"{SimpleUrl}\", \"type\" : \"{Type}\"}}";
		Debug.Log(postBody);
		using (var req = new UnityWebRequest(updateUrlApi, "POST"))
		{
			AddTokenHeader(req);
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

}
