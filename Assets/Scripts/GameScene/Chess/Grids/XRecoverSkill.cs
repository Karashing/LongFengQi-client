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
public class XRecoverSkill : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XGrid grid;
    public float recover_hp_rate = 1; // [0,1]
    public XRecoverSkill(XGrid xgrid, int xskill_id) : base(xgrid, xskill_id) {
        grid = xgrid;
        name = () => "恢复";
        role = () => $"立即将当前棋子复活并恢复最大生命值上限<color=red><b>{(int)(recover_hp_rate * 100f)}%</b></color>的血量";
        symbol_sprite_name = () => "heart-plus";
    }
    public override bool IsEnable() {
        if (grid.bind_chess != null) {
            return true;
        }
        return false;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        return is_confirm;
    }
    public override void Execute(XExtraData data) {
        bool is_revive = grid.bind_chess.hp == 0;
        var delta_hp = Mathf.CeilToInt(recover_hp_rate * grid.bind_chess.max_hp);
        grid.bind_chess.BeRestore(delta_hp);
        if (is_revive) grid.bind_chess.EnterActionQueue(0.5f);
    }
}