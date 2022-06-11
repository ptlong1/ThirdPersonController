using System;
using System.Collections;
using System.Collections.Generic;
using Assets._Project._Scripts.Screen;
using UnityEngine;
using ScriptableObjectArchitecture;

namespace Assets._Project._Scripts.DynamicData
{
	[Serializable]
	public class DynamicData
	{
		public string id;
		public string url;
		public string type;
		public string token;
		public string hostName;
		// public string _id;
		// public string __v;

	}

	[CreateAssetMenu(fileName = "DynamicDataContainer", menuName = "ScriptableObjects/DynamicDataContainer")]
	[Serializable]
	public class DynamicDataContainer : ScriptableObject
	{
		public List<DynamicData> dynamicDatas;
		public GameEvent OnFinishReplaceUrl;

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
			if (string.IsNullOrWhiteSpace(jsonFromServer)) return;
			string json = fixJson(jsonFromServer);
			JsonUtility.FromJsonOverwrite(json, this);
		}

		[ContextMenu("Find And Replace Url")]
		public void FindAndReplaceUrl()
		{
			ConferenceScreen[] screens = FindObjectsOfType<ConferenceScreen>(true);
			foreach (ConferenceScreen screen in screens)
			{
				string id = screen.GetComponent<ConferenceObjectData>().id;
				foreach (DynamicData data in dynamicDatas)
				{
					if (data.id.Equals(id))
					{
						screen.urlContent = data.url;
						screen.token = data.token;
						screen.hostName = data.hostName;
						if (!Enum.TryParse(data.type, out screen.screenType))
						{
							Debug.Log($"{id} wrong enum type {data.type}");
						}
					}
				}
				if (screen.defaultOn)
				{
					screen.ShowContent();
				}
				// conference banner

			}

			// if (OnFinishReplaceUrl != null)
			// {
			// 	OnFinishReplaceUrl.Raise();
			// 	Debug.Log("Raise OnFinishReplaceUrl");
			// }
		}
	}
}