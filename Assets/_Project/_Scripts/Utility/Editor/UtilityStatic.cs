using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using Assets._Project._Scripts.DynamicData;


public class UtilityStatic
{
	[MenuItem("Tools/Delete Pre Id")]
	public static void DeletePreId()
	{
		ConferenceObjectData[] conferenceObjectDatas = GameObject.FindObjectsOfType<ConferenceObjectData>();
		foreach(ConferenceObjectData data in conferenceObjectDatas)
		{
			string dataId = data.id;
			while (dataId.Length > 0 && dataId[0] != '-')
			{
				dataId.Remove(0, 1);
			}
			data.id = dataId;
		}
	}
	[MenuItem("Tools/Setup Pre Id")]
	public static void SetUpPreId()
	{
		ConferenceObjectData[] conferenceObjectDatas = GameObject.FindObjectsOfType<ConferenceObjectData>();
		foreach(ConferenceObjectData data in conferenceObjectDatas)
		{
			Undo.RecordObject(data, "Setup Pre Id");

			string dataId = data.id;

			string sceneName = data.gameObject.scene.name;
			data.id = sceneName + '-' + dataId;	
			PrefabUtility.RecordPrefabInstancePropertyModifications(data);
		}
	}
}
