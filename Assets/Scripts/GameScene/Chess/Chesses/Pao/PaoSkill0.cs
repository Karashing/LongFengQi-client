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
using System.Threading.Tasks;



[Serializable]
public class PaoSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public PaoSkill0(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 1f;
        name = () => "装填弹药";
        role = () => $"<b>消耗1★</b><br>使自身获得一枚可发射出去的炸药，最多储存3枚";
        symbol_sprite_name = () => "cannon";
    }
    public override bool IsEnable() {
        var buff = chess.GetBuff<PaoSkill0Buff>();
        if (buff != null) {
            if (buff.shell_num >= 3)
                return false;
        }
        if (GameInfo.bean >= 1)
            return true;
        else
            return false;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        return is_confirm;
    }
    public override void Execute(XExtraData data) {
        if (chess.camp == XCamp.SELF) GameInfo.bean -= 1;

        var xbuff = chess.GetBuff<PaoSkill0Buff>();
        if (xbuff != null) {
            xbuff.AddShell();
        }
        else {
            chess.owner_buffs.Add(new PaoSkill0Buff(chess, chess));
        }
        var effect = FM.LoadEffect("pao_skill0_effect");
        effect.transform.position = chess.transform.position;
        effect.Play();
    }
}