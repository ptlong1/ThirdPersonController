using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wolf3D.ReadyPlayerMe.AvatarSDK;
using ScriptableObjectArchitecture;
using Mirror;
public class RuntimeAvatarLoader : NetworkBehaviour
{
    public Transform mainPlayer;
    public GameObject defaultAvatar;
    public string[] avatarURLs;
	[SyncVar]
	public int avatarIndex;
	string avatarURL;
    public GameEvent OnLoadingAvatar;
    public GameEvent OnLoadedAvatar;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Started loading avatar. [{Time.timeSinceLevelLoad:F2}]");
		avatarURL = avatarURLs[ avatarIndex % avatarURLs.Length];
        LoadNewAvatar();
    }
    public void LoadNewAvatar()
    {
        AvatarLoader avatarLoader = new AvatarLoader();
        avatarLoader.LoadAvatar(avatarURL, OnAvatarImported, OnAvatarLoaded);
		if (isLocalPlayer)
		{
			OnLoadingAvatar.Raise();
		}
    }
    private void OnAvatarImported(GameObject avatar)
    {
        Debug.Log($"Avatar imported. [{Time.timeSinceLevelLoad:F2}]");
    }

    private void OnAvatarLoaded(GameObject avatar, AvatarMetaData metaData)
    {
        Debug.Log($"Avatar loaded. [{Time.timeSinceLevelLoad:F2}]\n\n{metaData}");
        // defaultAvatar.SetActive(false);
        AddArmature(avatar);
		if (isLocalPlayer)
		{
			OnLoadedAvatar.Raise();
		}
    }

    void AddArmature(GameObject avatar)
    {
        avatar.transform.parent = mainPlayer;
        avatar.transform.localPosition = Vector3.zero;
        avatar.transform.localEulerAngles = Vector3.zero;
        avatar.transform.localScale = Vector3.one;
        Destroy(avatar.GetComponent<Animator>());
        Animator mainAnimator = mainPlayer.GetComponent<Animator>();
        mainAnimator.Rebind();
    }
}
