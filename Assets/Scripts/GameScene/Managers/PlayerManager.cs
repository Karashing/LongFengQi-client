using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : BaseBehaviour {
    private static PlayerManager _instance;
    public static PlayerManager Ins {
        get {
            return _instance;
        }
    }
    void Awake() {
        _instance = this;
    }


}
