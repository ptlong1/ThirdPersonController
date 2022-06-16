using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class EnvironmentVariables
{
	public string urlLogin;
	public string resourceUrl;
	public string conferenceUrl;
   	public string webServerUrl;
}


[CreateAssetMenu(fileName = "EnvironmentVariablesContainer", menuName = "ScriptableObjects/EnvironmentVariablesContainer")]
public class EnvironmentVariablesContainer : ScriptableObject
{
	public  EnvironmentVariables environmentVariables;
	public string environmentUrl;

	[ContextMenu("Parse")]
	public void Parse(string resultJson)
	{
		environmentVariables = JsonUtility.FromJson<EnvironmentVariables>(resultJson);
	}

}
