using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project._Scripts.DynamicData
{
	public class DynamicDataManager : MonoBehaviour
	{
		public DynamicDataContainer dataContainer;
		public UserResponse userResponse;
		public ConferenceData conferenceData;
		public EnvironmentVariablesContainer environmentVariablesContainer;
		public string jsonUrl;
		public string jsonData;
		public bool parseOnStart;
		// Start is called before the first frame update

		private void Start()
		{
			if (parseOnStart)
			{
				TryGetAndParseData();
			}
		}

		public void TryGetAndParseData()
		{
			StartCoroutine(CR_TryGetAndParseData());
		}
		public IEnumerator CR_TryGetAndParseData()
		{
			yield return CR_TryGetJsonData(jsonUrl);
			dataContainer.ParseJsonToList(jsonData);
			dataContainer.FindAndReplaceUrl();
		}

		IEnumerator CR_TryGetJsonData(string url)
		{
			yield return StartCoroutine(WebServerAPI.CR_GetResouceJson(
				userResponse.token,
				environmentVariablesContainer.environmentVariables.conferenceUrl,
				conferenceData.ConferenceId));
			jsonData = WebServerAPI.Result;
		}
	}
}