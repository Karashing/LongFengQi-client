using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ToolI;
using DG.Tweening;
using UnityEngine.Events;
using UnityEditor;
using Unity.VisualScripting;



[Serializable]
public class XiaMoveSkill : XMoveSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private int mode {
        get {
            if (chess.HaveBuff<XiaSkill0Buff>()) {
                move_range = 3;
                return 1;
            }
            else {
                move_range = 2;
                return 0;
            }
        }
    }
    public XiaMoveSkill(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        name = () => {
            if (mode == 0) return "移动";
            else return "瞬步";
        };
    }
}