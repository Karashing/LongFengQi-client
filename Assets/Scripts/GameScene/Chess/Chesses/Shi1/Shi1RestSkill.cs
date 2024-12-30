using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using UnityEditor;



[Serializable]
public class Shi1RestSkill : XRestSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public Shi1RestSkill(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        role = () => $"原地回复<color=red><b>1</b></color>点血量，并为自身添加一层可抵挡<color=#fb9725><b>{chess.level + 1}</color></b>点伤害的<b>兽王盾</b>，最多叠加2层";
    }
    public override void Execute(XExtraData data) {
        base.Execute(data);
        var xbuff = chess.GetBuff<Shi1RestSkillBuff>();
        if (xbuff != null) {
            xbuff.Execute();
        }
        else {
            chess.owner_buffs.Add(new Shi1RestSkillBuff(chess, chess));
        }
    }
}