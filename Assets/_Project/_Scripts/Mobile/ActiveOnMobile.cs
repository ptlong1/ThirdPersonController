using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class ActiveOnMobile : MonoBehaviour
{
	#if UNITY_WEBGL && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern bool IsMobileBrowser();
	#endif


	public static bool CheckMobileBrowser()
	{	
		#if UNITY_EDITOR
    		return false; // value to return in Play Mode (in the editor)
		#elif UNITY_WEBGL
    		return IsMobileBrowser(); // value based on the current browser
		#else
    		return false; // value for builds other than WebGL
		#endif
  	}

	void OnEnable()
	{
		if (CheckMobileBrowser())
		{
			gameObject.SetActive(true);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
}
