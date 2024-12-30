using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// [Serializable]
// public class InteractData : BaseGame {
//     public server
//     public InteractData(Vector3Int xposition, GridType grid_type, int xlevel, XCamp xcamp) {
//         type = grid_type;
//         level = xlevel;
//         position = xposition;
//         camp = xcamp;
//     }
// }


// [Serializable]
// public class SkillData : BaseGame {
//     public int round;
//     public string server_id;
//     public int action_id;
//     public XExtraData extra_data; // Json
//     public SkillData(int xround, float xeffect_time, string xserver_id, int xaction_id, XExtraData xextra_data = null) {
//         round = xround;
//         effect_time = xeffect_time;
//         server_id = xserver_id;
//         action_id = xaction_id;
//         extra_data = xextra_data;
//     }
// }
[Serializable]
public class ActionData : BaseGame {
    public int round;
    public int action_id;
    public float effect_time;
    public string server_id;
    public int skill_code;
    public XExtraData extra_data; // Json
    public ActionData(int xround, int xaction_id, float xeffect_time, string xserver_id, int xskill_code, XExtraData xextra_data = null) {
        round = xround;
        action_id = xaction_id;
        effect_time = xeffect_time;
        server_id = xserver_id;
        skill_code = xskill_code;
        extra_data = xextra_data;
    }
}
[Serializable]
public class AddExtraActionData : BaseGame {
    public int round;
    public int action_id;
    public string server_id;
    public List<int> skill_codes;
    public List<XExtraData> extra_datas;
    public AddExtraActionData(int xround, int xaction_id, string xserver_id, List<int> xskill_codes, List<XExtraData> xextra_datas) {
        round = xround;
        action_id = xaction_id;
        server_id = xserver_id;
        skill_codes = xskill_codes;
        extra_datas = xextra_datas;
        if (xskill_codes.Count != xextra_datas.Count) {
            Debug.LogError($"skill_codes.Count: {skill_codes.Count} != extra_datas.Count: {extra_datas.Count}");
        }
    }
    public AddExtraActionData(XAction xaction, List<XExtraData> xextra_datas = null) {
        round = xaction.round;
        action_id = xaction.action_id;
        server_id = xaction.actor.server_id;
        skill_codes = new List<int>();
        extra_datas = new List<XExtraData>();
        foreach (var xskill in xaction.skills) {
            skill_codes.Add(xskill.skill_id);
            extra_datas.Add(new XExtraData());
        }
        if (xextra_datas != null) extra_datas = xextra_datas;
        if (skill_codes.Count != extra_datas.Count) {
            Debug.LogError($"skill_codes.Count: {skill_codes.Count} != extra_data.Count: {extra_datas.Count}");
        }
    }
    public XAction GetXAction() {
        var xaction = new XAction(server_id, skill_codes, round, action_id);
        Debug.Log(xaction.skills.Count);
        Debug.Log(skill_codes.Count);
        Debug.Log(extra_datas);
        Debug.Log(extra_datas.Count);
        for (int i = 0; i < xaction.skills.Count; ++i) {
            var xskill = xaction.skills[i];
            xskill.extra_data = extra_datas[i];
        }
        return xaction;
    }
}
[Serializable]
public class NextRoundData : BaseGame {
    public int round;
    public NextRoundData(int xround) {
        round = xround;
    }
}
[Serializable]
public class NextActionData : BaseGame {
    public int round;
    public int action_id;
    public NextActionData(int xround, int xaction_id) {
        round = xround;
        action_id = xaction_id;
    }
}

[Serializable]
public class GameResultData : BaseGame {
    public float game_score;
    public float total_mil;
    public GameResultData(float xgame_score, float xtotal_mil) {
        game_score = xgame_score;
        total_mil = xtotal_mil;
    }
}
[Serializable]
public class GameRatingData : BaseGame {
    public int rating;
    public int delta_rating;
    public GameRatingData(int xrating, int xdelta_rating) {
        rating = xrating;
        delta_rating = xdelta_rating;
    }
}
[Serializable]
public class ConversationData : BaseGame {
    public string user_name;
    public int emotion;
    public string sentence;
    public ConversationData(int xemotion, string xuser_name = null) {
        emotion = xemotion;
        if (xuser_name == null) user_name = GrpcService.Ins.user_info.nick_name;
    }
    public ConversationData(string xsentence, string xuser_name = null) {
        sentence = xsentence;
        if (xuser_name == null) user_name = GrpcService.Ins.user_info.nick_name;
    }
}