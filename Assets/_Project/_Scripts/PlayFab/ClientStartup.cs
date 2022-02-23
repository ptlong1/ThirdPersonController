using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using static PlayFab.PlayFabMultiplayerAPI;
using PlayFab.MultiplayerModels;
using PlayFab.Networking;
using Mirror.SimpleWeb;
public class ClientStartup : MonoBehaviour
{
    [Header("Address info")]
    public string IpV4Address;
    public ushort port;
    [Header("Build Info")]
    public string buildID;
    public string sessionID;
    public List<string> regions;
    void Start()
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true,
            CustomId = SystemInfo.deviceUniqueIdentifier
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnPlayFabLoginSuccess, OnLoginError);
    }

	private void OnLoginError(PlayFabError playFabError)
	{
        Debug.Log(playFabError);
	}

	private void OnPlayFabLoginSuccess(LoginResult loginResult)
	{
        Debug.Log("Login Success");
        // FindServer();
        // ConnectServer(IpV4Address, port);
        RequestServer();
	}

    void RequestServer()
    {
        RequestMultiplayerServerRequest request = new RequestMultiplayerServerRequest()
        {
            BuildId = buildID,
            SessionId = sessionID,
            PreferredRegions = regions
        };
        PlayFabMultiplayerAPI.RequestMultiplayerServer(request, OnRequestMultiplayerServer, OnRequestMultiplayerServerError);
    }

	private void OnRequestMultiplayerServer(RequestMultiplayerServerResponse response)
	{
        if (response == null)
        {
            Debug.Log("Response null");
            return;
        }
        Debug.Log("****THERE IS YOUR DETAILS****");
        Debug.Log($"- IP:  {response.IPV4Address}");
        Debug.Log($"- Port:  {response.Ports[0].Num}");
        ConnectServer(response.IPV4Address, (ushort) response.Ports[0].Num);
	}

	private void OnRequestMultiplayerServerError(PlayFabError playFabError)
	{
        Debug.Log("Request fail");
        Debug.Log(playFabError.ErrorMessage);
        Debug.Log(playFabError.ErrorDetails);
	}

	private void ConnectServer(string IPV4Address, ushort port)
	{
        MultiplayerNetworkServer.Instance.networkAddress = IPV4Address;
        MultiplayerNetworkServer.Instance.GetComponent<SimpleWebTransport>().port = port;
        MultiplayerNetworkServer.Instance.StartClient();
	}

	private void FindServer()
	{  
        ListMultiplayerServersRequest request = new ListMultiplayerServersRequest()
        {
            BuildId = "3ee15bf9-1a49-4caf-ad1e-4f74866ab76d",
            Region = "NorthEurope"
        };
        ListMultiplayerServers(request, OnListServerSuccess, OnListServerError);
	}

	private void OnListServerSuccess(ListMultiplayerServersResponse response)
	{
        Debug.Log("List Server Success");
        Debug.Log(response);
        Debug.Log(response.MultiplayerServerSummaries);
	}

	private void OnListServerError(PlayFabError playFabError)
	{
        Debug.Log("List Server Error");
        Debug.Log(playFabError);
	}
}
