using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UserInfo
{
	public string username;
	public string email;
	public string avatarUrl;
}

[CreateAssetMenu(fileName = "UserResponse", menuName = "ScriptableObjects/UserResponse")]
[Serializable]
public class UserResponse: ScriptableObject
{
	public UserInfo user;
	public string token;
	public string testRes;
	[ContextMenu("Print Json")]
	public void PrintJson()
	{
		Debug.Log(JsonUtility.ToJson(this, true));
	}

	string fixJson(string value)
	{
 	//    value = "{\"user\":" + value + "}";
 	   return value;
	}

	[ContextMenu("Parse Json")]
	public void TestParseJson()
	{
		ParseJson(testRes);
	}
	public void ParseJson(string jsonFromServer)
	{
		JsonUtility.FromJsonOverwrite(jsonFromServer, this);
		Debug.Log(jsonFromServer);
		// user = userTemp.user;
		// token = userTemp.token;
	}
}

