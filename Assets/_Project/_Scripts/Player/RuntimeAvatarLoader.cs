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
    // public GameObject defaultAvatar;
    // public string[] avatarURLs;
	// [SyncVar]
	// public int avatarIndex;
	public string defaultAvatar;
	[SyncVar]
	public string userAvatar;
	public bool useMeshSimplify = false;
	public float quality = 0.2f;
	string avatarURL;
    public GameEvent OnLoadingAvatar;
    public GameEvent OnLoadedAvatar;
	AvatarLoader avatarLoader;
    // Start is called before the first frame update
    void Start()
    {
		avatarLoader = new AvatarLoader();
        Debug.Log($"Started loading avatar. [{Time.timeSinceLevelLoad:F2}]");
		// Debug.Log("Assign Avatar");
    }
	
	[Command]
	public void CmdSetUserAvatar(string url)
	{
		if (String.IsNullOrEmpty(url) || String.IsNullOrWhiteSpace(url))
		{
			userAvatar = defaultAvatar;
		}
		else
		{
			userAvatar = url;
		}
		Debug.Log("CmdSetUserAvatar");
		avatarURL = userAvatar;
		RpcLoadNewAvatar(avatarURL);
	}

	[ClientRpc]
	void RpcLoadNewAvatar(string url)
	{
		avatarURL = url;
        LoadNewAvatar();
	}
	
	public void TestAvatar()
	{
        // AvatarLoader avatarLoader = new AvatarLoader();
        avatarLoader.LoadAvatar(avatarURL, OnAvatarImported, OnAvatarLoaded);
		
	}
    public void LoadNewAvatar()
    {
        // AvatarLoader avatarLoader = new AvatarLoader();
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
		if (useMeshSimplify)
			Simplify(avatar);
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

	void Simplify(GameObject avatar)
	{
		SkinnedMeshRenderer[] skinnedMeshes = avatar.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer skinnedMesh in skinnedMeshes)
		{
			SimplifySkinnedMesh(skinnedMesh);
		}
	}

	private void SimplifySkinnedMesh(SkinnedMeshRenderer skinnedMesh)
	{
		Mesh sourceMesh = skinnedMesh.sharedMesh;
		if (sourceMesh == null)
		{
			return;
		}
		var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(sourceMesh);

        // This is where the magic happens, lets simplify!
        meshSimplifier.SimplifyMesh(quality);

        // Create our final mesh and apply it back to our mesh filter
        skinnedMesh.sharedMesh = meshSimplifier.ToMesh();
		Destroy(sourceMesh);

	}
}
