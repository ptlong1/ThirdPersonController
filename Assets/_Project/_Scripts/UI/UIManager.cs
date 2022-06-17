using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PTL.UI;

public class UIManager : MonoBehaviour
{
    [Header("Model Window")]
    public ModelWindow modelWindowPrefab;
    
    [Header("Loading Avatar Model Window")]
    public Sprite loadingSprite;
    ModelWindow loadingModelWindow;
	public LoadingScreen loadingScreen;
	
	[ContextMenu("Show Loading Avatar Model Window")]
    public void ShowLoadingAvatarModelWindow()
    {
        loadingModelWindow = Instantiate(modelWindowPrefab, transform);
        loadingModelWindow.Initalize("Ready Player Me Avatar", false, loadingSprite, "Loading Player Avatar...");
        loadingModelWindow.Show();
    }

    public void HideLoadingAvatarModelWindow()
    {
		if (loadingModelWindow != null)
			loadingModelWindow.Hide();
    }

	public void ToggleLoadingScreen(bool vl)
	{
		loadingScreen.gameObject.SetActive(vl);
	}
}
