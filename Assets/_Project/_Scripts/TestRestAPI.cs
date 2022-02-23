using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class TestRestAPI : MonoBehaviour
{
    public string url;
    // Start is called before the first frame update
    void Start()
    {
    //    string url = "https://jsonplaceholder.typicode.com/posts/1";
       StartCoroutine(CallAPI(url));
    }

    IEnumerator CallAPI(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            Debug.Log(webRequest.downloadHandler.text);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
