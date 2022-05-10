using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
// using UnityEngine.AddressableAssets;
public class Bootstrap : MonoBehaviour
{
	[Scene]
	public string offlineScene;
    // Start is called before the first frame update
    void Start()
    {
		// Addressables.LoadSceneAsync(offlineScene);
    }

}
