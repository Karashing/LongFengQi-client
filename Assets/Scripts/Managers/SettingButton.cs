using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingButton : BaseBehaviour {
    public void OnClick() {
        ScreenManager.Ins.OnSetOn(true);
    }
}

