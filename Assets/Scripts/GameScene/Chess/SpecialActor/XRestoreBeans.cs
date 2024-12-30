using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using UnityEditor;


// FIXME
[Serializable]
public class XRestoreBeansSkill : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private TianActor tian_actor;
    public XRestoreBeansSkill(TianActor xtian_actor, int xskill_id) : base(xtian_actor, xskill_id) {
        tian_actor = xtian_actor;
        name = () => "生产";
        role = () => "生产一定量的资源，如果在自身回合内，田存在棋子，则该棋子一方获得一定量的资源";
        symbol_sprite_name = () => "maze-cornea";
    }
    private List<XGrid> grids;
    public override bool IsEnable() {
        int res_count = 0;
        grids = new List<XGrid>();
        var xgrids = GameInfo.GetGrids(GridType.TIAN);
        foreach (var xgrid in xgrids) {
            if (xgrid.state == GridState.HAVING) {
                grids.Add(xgrid);
                res_count += 1;
            }
        }
        Debug.Log("res_count: " + res_count);
        return res_count > 0;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        foreach (var xgrid in grids) {
            extra_data.AddSelectPositions(xgrid.grid_position);
        }
        return is_confirm;
    }
    public override void Execute(XExtraData data) {
        foreach (var xpos in data.select_positions) {
            var xgrid = GameInfo.grid_dict[xpos];
            xgrid.skills[skill_id].Execute(data);
        }
    }
}