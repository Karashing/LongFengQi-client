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
public class XMaintainSkill : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XGrid grid;
    public XMaintainSkill(XGrid xgrid, int xskill_id) : base(xgrid, xskill_id) {
        grid = xgrid;
        name = () => "维修";
        role = () => "回复<color=red><b>2</b></color>点血量";
        symbol_sprite_name = () => "thor-hammer";
    }
    public override bool IsEnable() {
        return true;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        return is_confirm;
    }
    public override void Execute(XExtraData data) {
        if (grid.have_hp)
            grid.hp = Mathf.Min(grid.hp + 2, grid.max_hp);
        grid.multi_mil *= 0.5f;
    }
}