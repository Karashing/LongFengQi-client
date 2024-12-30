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
public class TaSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XGrid grid;
    public TaSkill0(XGrid xgrid, int xskill_id) : base(xgrid, xskill_id) {
        grid = xgrid;
        name = () => "解封";
        role = () => "塔之余辉降临尘世。一为龙，二为凤";
        effect_time = () => 1.5f;
        symbol_sprite_name = () => "thor-hammer";
    }
    public override bool IsEnable() {
        return true;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        return is_confirm;
    }
    public override void Execute(XExtraData data) {
        var xgrid = grid as TaGrid;
        var effect = FM.LoadEffect("ta_skill0_effect", () => {
            xgrid.phase += 1;
            if (xgrid.phase <= 5) {
                xgrid.max_hp = (xgrid.phase / 2 + 1) * 10;
            }
            else {
                xgrid.max_hp = (xgrid.phase / 2 + 1) * 5 + 15;
            }
            xgrid.hp = xgrid.max_hp;
            if (xgrid.effect == null) {
                xgrid.effect = FM.LoadEffect("ta_effect" + Mathf.Clamp(xgrid.phase / 2, 0, 2).ToString()) as TaEffect;
                xgrid.effect.SetParent(xgrid.transform);
                xgrid.effect.Play();
            }
            xgrid.QuitActionQueue();
        });
        effect.transform.position = GM.grid_map.GetCellCenterWorld(xgrid);
        effect.Play();
    }
    //922
}