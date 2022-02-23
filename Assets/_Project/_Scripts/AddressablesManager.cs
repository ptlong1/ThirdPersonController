using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;
using System;

[Serializable]
public class AssetReferenceAudioClip: AssetReferenceT<AudioClip>
{
    public AssetReferenceAudioClip(string guid): base(guid)
    {
    }
}
public class AddressablesManager : MonoBehaviour
{
    [SerializeField]
    AssetReference ARplayerAmature;
    [SerializeField]
    AssetReferenceAudioClip ARmusic;
    [SerializeField]
    AssetReferenceTexture2D ARlogo;

    public RawImage logoHolder;

    CinemachineFreeLook virtualCam;
    GameObject playerController;
    // Start is called before the first frame update
    void Start()
    {

       Debug.Log("Initalizing Addressable");
       Addressables.InitializeAsync().Completed += AddressablesManager_Completed; 
    }

	private void AddressablesManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
	{
       Debug.Log("Initalized Addressable");
        ARplayerAmature.InstantiateAsync().Completed += (go) => 
        {
            playerController = go.Result;
            Debug.Log("Instantiated Player");
        };
        // ARmusic.LoadAssetAsync<AudioClip>().Completed += (clip) =>
        // {
        //     AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        //     audioSource.clip = clip.Result;
        //     audioSource.playOnAwake = false;
        //     audioSource.loop = true;
        //     audioSource.Play();
        //     Debug.Log("Loaded audio clip");
        // };
        ARlogo.LoadAssetAsync<Texture2D>();
        Debug.Log("Loaded Asset");

	}

	// Update is called once per frame
	void Update()
    {
        if (ARlogo.Asset != null && logoHolder.texture == null)
        {
            logoHolder.texture = ARlogo.Asset as Texture2D;
            Debug.Log("Logo loaded with raw iamge"); 
        }
    }

    // private void OnDestroy() {
    //     ARplayerAmature.ReleaseInstance(playerController);
    //     ARlogo.ReleaseAsset();

    // }
}
