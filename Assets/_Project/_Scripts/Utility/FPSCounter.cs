using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FPSCounter : MonoBehaviour
{

	public TMP_Text fpsCounter;
	public float sec;
	IEnumerator CR_CountFPS()
	{
		while (true)
		{
			yield return new WaitForSeconds(sec);
			int fps = (int)Mathf.Round(1/Time.deltaTime);
			fpsCounter.text = fps.ToString();
		}
	}
	private void Start() {
		StartCoroutine(CR_CountFPS());
	}
}
