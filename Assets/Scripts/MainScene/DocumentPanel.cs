using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public class DocumentPanel : MonoBehaviour {
    private static DocumentPanel _instance;
    public static DocumentPanel Ins {
        get {
            return _instance;
        }
    }
    public GameObject document_panel_go;

    public void OnShow() {
        document_panel_go.SetActive(true);
    }
    public void OnClose() {
        document_panel_go.SetActive(false);
    }
}