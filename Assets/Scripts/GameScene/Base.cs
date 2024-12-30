using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseBehaviour : MonoBehaviour {
    protected static GameManager GM {
        get {
            return GameManager.Ins;
        }
    }
    protected static FactoryManager FM {
        get {
            return FactoryManager.Ins;
        }
    }
    protected static PlayerManager PM {
        get {
            return PlayerManager.Ins;
        }
    }
    protected static EventManager EM {
        get {
            return EventManager.Ins;
        }
    }
    protected static NetworkManager NM {
        get {
            return NetworkManager.Ins;
        }
    }
    protected static GameInfo GameInfo {
        get {
            return GameManager.Ins.game_info;
        }
    }
    protected static GameData GameData {
        get {
            return GameManager.Ins.game_data;
        }
    }
}
public class BaseGame {
    protected static GameManager GM {
        get {
            return GameManager.Ins;
        }
    }
    protected static FactoryManager FM {
        get {
            return FactoryManager.Ins;
        }
    }
    protected static PlayerManager PM {
        get {
            return PlayerManager.Ins;
        }
    }
    protected static EventManager EM {
        get {
            return EventManager.Ins;
        }
    }
    protected static NetworkManager NM {
        get {
            return NetworkManager.Ins;
        }
    }
    protected static GameInfo GameInfo {
        get {
            return GameManager.Ins.game_info;
        }
    }
    protected static GameData GameData {
        get {
            return GameManager.Ins.game_data;
        }
    }
}
