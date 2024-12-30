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
public class TaSkill1 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    public XActor source_actor;
    private TaGrid grid;
    public TaSkill1(TaGrid xgrid, int xskill_id) : base(xgrid, xskill_id) {
        grid = xgrid;
        name = () => "赐福";
        role = () => "塔之余辉降临尘世。一为龙，二为凤";
        effect_time = () => 2f;
        symbol_sprite_name = () => "thor-hammer";
    }
    public override bool IsEnable() {
        return true;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        extra_data.new_actors.Add(new(new XActorData(
            serverId: Generate.GenerateId(),
            xlevel: grid.level,
            xcamp: source_actor.camp
        )));
        return true;
    }
    public override void Execute(XExtraData data) {
        XActorData actor_data = data.new_actors[0].LoadData<XActorData>();
        Debug.Log("taskill1");
        var new_actor = FM.LoadTaBuffActor(actor_data, grid);
        var xchesses = new List<XActor>();

        foreach (var xchess in GameInfo.GetChesss(source_actor.camp)) {
            if (xchess.grid.in_board)
                xchesses.Add(xchess);
        }
        if (grid.phase == 1) {
            new_actor.owner_buffs.Add(new TaBuff0(new_actor, xchesses));
            grid.effect.Kill();
            grid.effect = null;
        }
        else if (grid.phase == 3) {
            new_actor.owner_buffs.Add(new TaBuff1(new_actor, xchesses));
            grid.effect.Kill();
            grid.effect = null;
        }
        else {
            new_actor.owner_buffs.Add(new TaBuff2(new_actor, xchesses));
        }
        grid.speed = Mathf.Min(grid.speed + 5, 40f);
        grid.phase += 1;
        var effect = FM.LoadEffect("ta_skill1_effect");
        effect.transform.position = GM.grid_map.GetCellCenterWorld(grid);
        effect.Play();
        grid.EnterActionQueue();
    }
}