using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using UnityEditor;
using ToolI;



[Serializable]
public class TaBuffActorSkill1 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private TaBuffActor actor;
    public TaBuffActorSkill1(TaBuffActor xactor, int xskill_id) : base(xactor, xskill_id) {
        actor = xactor;
        name = () => "龙凤之力";
        role = () => $"对指定单位造成其{20}%最大生命值（最高不超过{5}点）的真实伤害";
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
        var xgrid = GameInfo.grid_dict[data.select_positions[^1]];
        var effect = FM.LoadEffect("ta_buff_actor_skill1_effect", () => {
            if (xgrid.have_hp) {
                var atk = Mathf.Min((int)(0.3f * xgrid.max_hp), 6);
                actor.grid.Attack(atk, xgrid);
            }
            else {
                var atk = Mathf.Min((int)(0.3f * xgrid.bind_chess.max_hp), 6);
                actor.grid.Attack(atk, xgrid.bind_chess);
            }
        }) as TaBuffActorSkill1Effect;
        var st_pos = GM.grid_map.GetCellCenterWorld(actor.grid);
        var ed_pos = GM.grid_map.GetCellCenterWorld(xgrid);
        effect.transform.position = st_pos;
        effect.target_pos = ed_pos;
        effect.distance = TileMap6.GetDistance(actor.grid.grid_position, xgrid.grid_position);
        effect.transform.eulerAngles = MathI.GetRotationFromVector(st_pos - ed_pos, 180);
        effect.Play();
    }
}