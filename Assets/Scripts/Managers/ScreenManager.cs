using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScreenManager : BaseBehaviour {
    private static ScreenManager _instance;
    public static ScreenManager Ins {
        get {
            return _instance;
        }
    }
    public bool isFullScreen {
        get {
            return isFullScreenToggle.isOn;
        }
        set {
            isFullScreenToggle.isOn = value;
        }
    }
    public int resolveId {
        get {
            return resolveDropdown.value;
        }
        set {
            resolveDropdown.value = value;
        }
    }

    public GameObject panelGo;
    public Toggle isFullScreenToggle;
    public TMP_Dropdown resolveDropdown;
    List<Vector2Int> resolveList = new List<Vector2Int> {
        new(3840,2160),
        new(2560,1440),
        new(1920,1080),
        new(1600,900),
        new(1280,720),
        new(960,540),
        new(720,405),
    };

    void Awake() {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start() {
        isFullScreen = true;
        resolveId = 2;
        Refresh();
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.F11) || Input.GetKeyDown(KeyCode.Escape)) {
            isFullScreen = !isFullScreen;
            Refresh();
        }
    }
    void Refresh() {
        if (isFullScreen)
            Screen.SetResolution(resolveList[0].x, resolveList[0].y, isFullScreen);
        else
            Screen.SetResolution(resolveList[resolveId].x, resolveList[resolveId].y, isFullScreen);
    }

    public void OnScreenChange() {
        Refresh();
    }
    public void OnSetOn(bool isOn) {
        panelGo.SetActive(isOn);
    }



}
