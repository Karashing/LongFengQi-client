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
public class XiaSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public XiaSkill0(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 1f;
        name = () => "侠客行";
        role = () => $"<b>消耗1★</b><br>使自身进入<b>侠客行</b>状态，持续<b>3</b>回合。<br><b>侠客行</b>期间，【移动】强化为【瞬步】，【普攻】强化为【摄伏诸恶】，并提升自身<color=#5cacee><b>80</color></b>点速度<br><b>侠客行</b>期间该技能无法再次使用";
        symbol_sprite_name = () => "ninja-heroic-stance";
    }
    public override bool IsEnable() {
        if (chess.HaveBuff<XiaSkill0Buff>()) {
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
        chess.owner_buffs.Add(new XiaSkill0Buff(chess, new List<XActor> { chess }));
        //TODO: 556
        var effect = FM.LoadEffect("xia_skill0_effect");
        effect.transform.position = GM.grid_map.GetCellCenterWorld(chess.grid);
        effect.Play();
    }
}