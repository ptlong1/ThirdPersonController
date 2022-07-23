using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Web;
using DG.Tweening;
using UnityEngine.Networking;
using Assets._Project._Scripts.Screen;
using Assets._Project._Scripts.DynamicData;
using EasyUI.Toast;
[Serializable]
public class UpdateResponse
{
	public string url; 
	public string token;
	public string type;
	public string hostName;

}

public class UpdateDataWindow : MonoBehaviour
{
	public TMP_Text headerId;
	private string id;
	public string Id
	{
		get {return id;}
		set { 
			id = value;
			headerId.text = "ID: " + id;
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
	public string HostName { get => hostName; set => hostName = value; }

	private string hostName;

	public GameObject updateTable;
	public string updateUrlApi;

	public UserResponse userResponse;
	public ConferenceData conferenceData;
	public TMP_InputField contentUrl;
	public TMP_InputField hostNameInputField;
	public ConferenceObjectData currentConferenceObject;
	public TMP_InputField currentUrl;
	public TMP_InputField currentType;
	public TMP_InputField currentHost;
	public EnvironmentVariablesContainer envContainer;
	
	// public Button

	// Start is called before the first frame update
	void Start()
    {
        
    }
	private void OnEnable() {
		bool vl = currentType.text == screenTypeEnum.Slide.ToString();
		
		currentHost.transform.parent.gameObject.SetActive(vl);
		hostNameInputField.transform.parent.gameObject.SetActive(vl);
		
	}

	[ContextMenu("Parse Url")]
	public void ParseUrlAndInputField()
	{
		var uri = new Uri(Url);
		var query = HttpUtility.ParseQueryString(uri.Query);
		// Debug.Log(query.Get("token"));
		// Debug.Log(query.Get("type"));
		// Token = query.Get("token");
		Token = userResponse.token;
		Type = query.Get("type");
		SimpleUrl = uri.GetLeftPart(UriPartial.Path);
		HostName = hostNameInputField.text;
		if (String.IsNullOrWhiteSpace(HostName))
		{
			HostName = userResponse.user.username;
		}
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
		ParseUrlAndInputField();
		if (String.IsNullOrWhiteSpace(Type) 
			|| String.IsNullOrWhiteSpace(SimpleUrl))
			{
				Debug.Log("Data not valid");
				return;
			}
		StartCoroutine(CR_UpdateData());
	}

	void UpdateSuccessCB(UpdateResponse response)
	{
		CloseUI();
		contentUrl.text = String.Empty;
		ConferenceScreen screen = currentConferenceObject.GetComponent<ConferenceScreen>();
		screen.urlContent = response.url;
		screen.token = response.token;
		screen.hostName = response.hostName;
		if (!Enum.TryParse(response.type, out screen.screenType))
		{
			Debug.Log($"{id} wrong enum type {response.type}");
		}
		screen.RestartContent();

	}
	IEnumerator CR_UpdateData()
	{
		// string postBody = $"{{\"id\": \"{Id}\",\"url\": \"{SimpleUrl}\", \"type\" : \"{Type}\"}}";
		string postBody = $"{{\"url\": \"{Url}\", \"type\" : \"{Type}\",\"conferenceId\": \"{conferenceData.ConferenceId}\",\"hostName\": \"{HostName}\"}}";
		Debug.Log(postBody);
		string newUpdateUrl = WebServerAPI.Combine(envContainer.environmentVariables.resourceUrl, Id + "/update");
		Debug.Log(newUpdateUrl);
		using (var req = new UnityWebRequest(newUpdateUrl, "POST"))
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
				Toast.Show("Update error", 2f, ToastColor.Red, ToastPosition.BottomCenter);
			}
			else
			{
				Debug.Log("Received: " + req.downloadHandler.text);
				UpdateResponse response = JsonUtility.FromJson<UpdateResponse>(req.downloadHandler.text);
				Toast.Show("Update success", 2f, ToastColor.Blue, ToastPosition.BottomCenter);
				UpdateSuccessCB(response);
			}
		}
	}

}
