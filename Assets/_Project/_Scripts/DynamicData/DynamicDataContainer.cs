using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DynamicData
{
	public string id;
	public string url;
	public string type;
	public string token;
}

[CreateAssetMenu(fileName = "DynamicDataContainer", menuName = "ScriptableObjects/DynamicDataContainer")]
[Serializable]
public class DynamicDataContainer : ScriptableObject
{
	public List<DynamicData> dynamicDatas;
	
	[ContextMenu("Print Json")]
	public void PrintJson()
	{
		Debug.Log(JsonUtility.ToJson(this, true));
	}

	string fixJson(string value)
	{
 	   value = "{\"dynamicDatas\":" + value + "}";
 	   return value;
	}


	public void ParseJsonToList(string jsonFromServer)
	{
		string json = fixJson(jsonFromServer);
		dynamicDatas = JsonUtility.FromJson<DynamicDataContainer>(json).dynamicDatas;
	}

	[ContextMenu("Find And Replace Url")]
	public void FindAndReplaceUrl()
	{
		Screen[] screens = FindObjectsOfType<Screen>(true);
		foreach(Screen screen in screens)
		{
			string id = screen.GetComponent<ConferenceData>().id;
			foreach (DynamicData data in dynamicDatas)
			{
				if (data.id.Equals(id))
				{
					screen.urlContent = data.url;
					screen.token = data.token;
					if (!Enum.TryParse<screenTypeEnum>(data.type, out screen.screenType))
					{
						Debug.Log($"{id} wrong enum type {data.type}");
					}
				}
			}
		}
	}
}
