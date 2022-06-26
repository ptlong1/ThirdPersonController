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
   	public string ttsUrl;
}


[CreateAssetMenu(fileName = "EnvironmentVariablesContainer", menuName = "ScriptableObjects/EnvironmentVariablesContainer")]
public class EnvironmentVariablesContainer : ScriptableObject
{
	public  EnvironmentVariables environmentVariables;
	public string environmentUrl;

	public string EnvironmentUrl { 
		get {
			string newUrl = environmentUrl + "?" + UnityEngine.Random.Range(100000, 999999).ToString();
			return newUrl;
		} 
		set => environmentUrl = value; }

	[ContextMenu("Parse")]
	public void Parse(string resultJson)
	{
		environmentVariables = JsonUtility.FromJson<EnvironmentVariables>(resultJson);
	}

}
