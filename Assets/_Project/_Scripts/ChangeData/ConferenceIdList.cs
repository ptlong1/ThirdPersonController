using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConferenceIdList : MonoBehaviour
{
	public GameObject content;
	public ConferenceIdButton buttonPrefab;

	public void Clear()
	{
		foreach(Transform child in content.transform)
		{
			Debug.Log("ABC");
			GameObject.Destroy(child.gameObject);
		}
	}
	public ConferenceIdButton AddButton(string id, int idx)
	{
		ConferenceIdButton newBtn = Instantiate(buttonPrefab, content.transform);
		newBtn.Id = id;
		newBtn.Idx = idx;
		return newBtn;
	}
}
