using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEditor;
using JetBrains.Annotations;
using System.ComponentModel;

[Serializable]
public enum ActorType {
    CHESS = 0,
    GRID = 1,
    SHOP = 2,
    TIAN_ACTOR = 3,
    TA_BUFF_ACTOR = 3,
    ACTOR = 100,
}
[Serializable]
public enum SymbolType {
    FengRao = 0,
    HuiMie = 1,
    JiYi = 2,
    XuWu = 3,
    XunLie = 4,
    HuanYu = 5,
    TongXie = 6,
    CunHu = 7,
    ZhiShi = 8,
    KaiTuo = 9,
}
public static class CampI {
    public static XCamp GetOppositeCamp(XCamp xcamp) {
        if (xcamp == XCamp.SELF) return XCamp.ENEMY;
        if (xcamp == XCamp.ENEMY) return XCamp.SELF;
        return xcamp;
    }
}
[Serializable]
public enum XTarget {
    ANY = 0,
    CHESS = 1,
    GRID = 2,
}
[Serializable]
public enum XCamp {
    SELF = 0,
    ENEMY = 1,
    NEUTRAL = 2,
    PUBLIC_ENEMY = 3,
}

[Serializable]
public class XAction : BaseGame {
    public XActor actor;
    public List<XSkill> skills;
    public int round, action_id;
    public XActionCard card;
    public string action_name;

    public XAction(string server_id, List<int> skill_codes, int xround, int xaction_id) {
        actor = GameInfo.actor_dict[server_id];
        action_name = actor.word;
        skills = new List<XSkill>();
        foreach (var skill_code in skill_codes) {
            skills.Add(actor.GetSkill(skill_code));
        }
        round = xround;
        action_id = xaction_id;
    }
    public XAction(XActor xactor, List<XSkill> xskills, int xround = 0, int xaction_id = 0) {
        actor = xactor;
        action_name = actor.word;
        skills = xskills;
        round = xround;
        action_id = xaction_id;
    }
    public string GetActionStr() {
        var res = "(" + round + ", " + action_id + ") " + actor.word + $": [";
        for (int i = 0; i < skills.Count; ++i) {
            var xskill = skills[i];
            res += xskill.skill_id;
            if (i < skills.Count - 1) res += ", ";
        }
        res += "]";
        return res;
    }
    public void SetActionName(string name) {
        action_name = name;
    }
}

[Serializable]
public class XExtraData : BaseGame {
    public List<string> actor_server_ids = new List<string>();
    // public List<string> grid_server_ids = new List<string>();
    [SerializeField]
    private List<Vector3Int> _select_positions = new List<Vector3Int>();
    public List<Vector3Int> select_positions {
        get {
            var _positions = new List<Vector3Int>();
            foreach (var xpos in _select_positions) {
                _positions.Add(GameInfo.GetLocalPosition(xpos));
            }
            return _positions;
        }
    }
    public void NewSelectPositions() {
        _select_positions = new List<Vector3Int>();
    }
    public void AddSelectPositions(Vector3Int pos) {
        _select_positions.Add(GameInfo.GetRealPosition(pos));
    }
    public List<ActorData> new_actors = new List<ActorData>();
    public List<float> ramdon_nums = new List<float>();
}


[Serializable]
public class XSkillDescribe {
    public string name;
    public string role;
    public Sprite symbol_sprite;
}

[Serializable]
public class XDescribe {
    public string name;
    public string role;
    public List<XSkillDescribe> skill; // TODO: Del
}


[Serializable]
public class ActorData : BaseGame {
    public int round;
    public ActorType actor_type;
    public string data; // JSON
    public ActorData(ChessData xchess) {
        round = GameInfo.cur_round;
        actor_type = ActorType.CHESS;
        data = JsonUtility.ToJson(xchess);
    }
    public ActorData(GridData xgrid) {
        round = GameInfo.cur_round;
        actor_type = ActorType.GRID;
        data = JsonUtility.ToJson(xgrid);
    }
    public ActorData(XActorData xactor) {
        round = GameInfo.cur_round;
        actor_type = ActorType.ACTOR;
        data = JsonUtility.ToJson(xactor);
    }
    public T LoadData<T>() {
        return JsonUtility.FromJson<T>(data);
    }
}

[Serializable]
public class XActorData : BaseGame {
    public string server_id;
    public int level;
    public int camp;
    public XCamp local_camp {
        get {
            return GameInfo.GetLocalCamp(camp);
        }
    }
    public XActorData(string serverId, int xlevel, XCamp xcamp = XCamp.NEUTRAL) {
        server_id = serverId;
        level = xlevel;
        camp = GameInfo.GetRealCamp(xcamp);
    }
}
