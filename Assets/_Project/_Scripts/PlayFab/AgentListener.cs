using System.Collections;
using UnityEngine;
using PlayFab;
using System;
using System.Collections.Generic;
using PlayFab.MultiplayerAgent.Model;
using PlayFab.Networking;

public class AgentListener : MonoBehaviour {
    private List<ConnectedPlayer> _connectedPlayers;
    public bool Debugging = true;
    // Use this for initialization
    void Start () {
#if UNITY_SERVER
        Debug.Log("UnityServer Code");
        _connectedPlayers = new List<ConnectedPlayer>();
        PlayFabMultiplayerAgentAPI.Start();
        PlayFabMultiplayerAgentAPI.IsDebugging = Debugging;
        PlayFabMultiplayerAgentAPI.OnMaintenanceCallback += OnMaintenance;
        PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnShutdown;
        PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
        PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnAgentError;

        MultiplayerNetworkServer.Instance.OnPlayerAdded.AddListener(OnPlayerAdded);
        MultiplayerNetworkServer.Instance.OnPlayerRemoved.AddListener(OnPlayerRemoved);

        StartCoroutine(ReadyForPlayers());
#endif
    }

    IEnumerator ReadyForPlayers()
    {
        yield return new WaitForSeconds(.5f);
        PlayFabMultiplayerAgentAPI.ReadyForPlayers();
    }
    
    private void OnServerActive()
    {
        MultiplayerNetworkServer.Instance.StartListen();
        Debug.Log("Server Started From Agent Activation");
    }

    private void OnPlayerRemoved(string playfabId)
    {
        ConnectedPlayer player = _connectedPlayers.Find(x => x.PlayerId.Equals(playfabId, StringComparison.OrdinalIgnoreCase));
        _connectedPlayers.Remove(player);
        PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
    }

    private void OnPlayerAdded(string playfabId)
    {
        _connectedPlayers.Add(new ConnectedPlayer(playfabId));
        PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
    }

    private void OnAgentError(string error)
    {
        Debug.Log(error);
    }

    private void OnShutdown()
    {
        Debug.Log("Server is shutting down");
        foreach(var conn in MultiplayerNetworkServer.Instance.Connections)
        {
            conn.Connection.Send<ShutdownMessage>(new ShutdownMessage());
        }
        StartCoroutine(Shutdown());
    }

    IEnumerator Shutdown()
    {
        yield return new WaitForSeconds(5f);
        Application.Quit();
    }

    private void OnMaintenance(DateTime? NextScheduledMaintenanceUtc)
    {
        Debug.LogFormat("Maintenance scheduled for: {0}", NextScheduledMaintenanceUtc.Value.ToLongDateString());
        foreach (var conn in MultiplayerNetworkServer.Instance.Connections)
        {
            conn.Connection.Send<MaintenanceMessage>(new MaintenanceMessage() {
                ScheduledMaintenanceUTC = (DateTime)NextScheduledMaintenanceUtc
            });
        }
    }
}