using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareResources : MonoBehaviour
{
	public EnvironmentVariablesController environmentVariablesController;
	public LoginWindow loginWindow;

    // Start is called before the first frame update
    void Awake()
    {
        environmentVariablesController.enabled = false;
		loginWindow.enabled = false;
    }

	IEnumerator Start(){
		environmentVariablesController.enabled = true;
		yield return environmentVariablesController.CR_ParseEnvironmentJson();
		loginWindow.enabled = true;
		yield return loginWindow.CR_GetAllConferenceMetaData();
	}
}
