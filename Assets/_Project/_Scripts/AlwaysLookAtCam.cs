using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysLookAtCam : MonoBehaviour {
    Camera mainCam;
    // Start is called before the first frame update
    void Start () {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate () {
        if (mainCam) {
            // Vector3 euler = Quaternion.LookRotation (-mainCam.transform.position + transform.position, Vector3.up).eulerAngles;
            Vector3 euler = Quaternion.LookRotation (mainCam.transform.forward, Vector3.up).eulerAngles;
            euler = new Vector3 (0, euler.y, 0);
            transform.rotation = Quaternion.Euler (euler);
        }
    }
}