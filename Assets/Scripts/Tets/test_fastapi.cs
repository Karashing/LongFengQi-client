using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Events;

public class test_fastapi : MonoBehaviour {
    public string m_ApiUrl = "http://129.211.6.217:38310";

    private void Start() {
        StartCoroutine(GetData(m_ApiUrl, (string data) => {
            Debug.Log($"Response: {data}");
        }));
    }

    IEnumerator GetData(string path, UnityAction<string> onGetJson) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(path)) {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success) {
                string json = webRequest.downloadHandler.text;
                onGetJson?.Invoke(json);
            }
            else {
                Debug.Log("error = " + webRequest.error + "\n Load Path = " + path);
                onGetJson?.Invoke(null);
            }
        }
    }


}
