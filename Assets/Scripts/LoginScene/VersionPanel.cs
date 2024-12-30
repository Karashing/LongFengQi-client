using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class VersionPanel : MonoBehaviour {
    public TMP_Text version_text;

    public void Start() {
        CheckVersion();
    }
    public void CheckVersion() {
        var result = GrpcService.Ins.Connect();
        if (!result.is_ok) {
            version_text.text = $"最新版本: {result.new_version}<br>当前版本: {result.cur_version}";
            gameObject.SetActive(true);
        }
        else {
            gameObject.SetActive(false);
        }
    }
    public void QuitButton() {
        Application.Quit();
    }
}