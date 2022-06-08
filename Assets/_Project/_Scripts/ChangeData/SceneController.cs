using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using ScriptableObjectArchitecture;

public class SceneController : MonoBehaviour
{
	[Scene]
	public string[] scenes;
	TMP_Dropdown dropdown;
	int currentIndex;
	public GameEvent OnLoadNewScene;
    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
		foreach(string scene in scenes)
		{
			// Debug.Log(GetSceneName(scene));
			string sceneName = GetSceneName(scene);
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(sceneName);
			dropdown.options.Add(optionData);
		}
		currentIndex = 0;
    }

	string GetSceneName(string scene)
	{
		string sceneName = scene.Split('/').Last().Split('.').First();
		return sceneName;
	}

	public void ChangeToScene(int idx)
	{
		StartCoroutine(CR_ChangeToScene(idx));
	}
	IEnumerator CR_ChangeToScene(int idx)
	{
		if (SceneManager.GetSceneByPath(scenes[currentIndex]).IsValid())
			yield return SceneManager.UnloadSceneAsync(GetSceneName(scenes[currentIndex]));
		currentIndex = idx;
		if (SceneManager.GetSceneByPath(scenes[currentIndex]).IsValid() == false)
			yield return SceneManager.LoadSceneAsync(GetSceneName(scenes[currentIndex]), LoadSceneMode.Additive);
		GameObject.FindGameObjectWithTag("Chat").SetActive(false);
		OnLoadNewScene.Raise();
	}
}
