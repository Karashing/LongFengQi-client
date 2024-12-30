using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using LitJson;
using UnityEngine;
using UnityEngine.Events;

public class InteractManager : BaseBehaviour {
    private static InteractManager _instance;
    public static InteractManager Ins {
        get {
            return _instance;
        }
    }
    void Awake() {
        _instance = this;
    }

    public void OnCheckActorDetail() {

    }

}