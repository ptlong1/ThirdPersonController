using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FPSCounter : MonoBehaviour
{

	public TMP_Text fpsCounter;
    // Update is called once per frame
    void Update()
    {
        float fps = 1/Time.deltaTime;
		fpsCounter.text = fps.ToString();
    }
}
