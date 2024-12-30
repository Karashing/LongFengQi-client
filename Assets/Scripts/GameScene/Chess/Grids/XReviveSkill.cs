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
public class XReviveSkill : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XGrid grid;
    public XReviveSkill(XGrid xgrid, int xskill_id) : base(xgrid, xskill_id) {
        grid = xgrid;
        name = () => "复活";
        role = () => "立即将当前重伤棋子复活，<br>并回复<color=red><b>1</b></color>点血量";
        symbol_sprite_name = () => "heart-revive";
    }
    public override bool IsEnable() {
        if (grid.bind_chess != null) {
            if (grid.bind_chess.hp == 0) {
                return true;
            }
        }
        return false;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        return is_confirm;
    }
    public override void Execute(XExtraData data) {
        grid.bind_chess.hp = 1;
        grid.bind_chess.EnterActionQueue(0.5f);
    }
}