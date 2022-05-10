using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicDataManager : MonoBehaviour
{
	public DynamicDataContainer dataContainer;
	public string jsonUrl;
	public string jsonData;
    // Start is called before the first frame update
    IEnumerator Start()
    {
		yield return TryGetJsonData(jsonUrl);
		dataContainer.ParseJsonToList(jsonData);
		dataContainer.FindAndReplaceUrl();
    }

	IEnumerator TryGetJsonData(string url)
	{
		yield return null;
	}
}
