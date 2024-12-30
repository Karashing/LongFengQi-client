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
public class GongRestSkill : XRestSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private int mode;
    private XChess chess;
    public GongRestSkill(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        name = () => {
            if (mode == 0) return "休息";
            else return "修缮";
        };
        role = () => {
            if (mode == 0) return "原地回复<color=red><b>1</b></color>点血量";
            else return $"为所处位置的<i>建筑物</i>恢复<color=red><b>{chess.cur_attack}</b></color>点血量";
        };
        symbol_sprite_name = () => "night-sleep";
    }
    public override void Init() {
        base.Init();
        mode = 0;
        var xgrid = chess.grid;
        if (xgrid.have_hp) {
            if (xgrid.hp < xgrid.max_hp) {
                mode = 1;
            }
        }
    }
    public override bool IsEnable() {
        return base.IsEnable();
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        return base.IsInteractEnd(is_confirm);
    }
    public override void Execute(XExtraData data) {
        if (mode == 0) {
            base.Execute(data);
            return;
        }
        chess.energy += 15;
        chess.grid.hp = Mathf.Min(chess.grid.hp + chess.cur_attack, chess.grid.max_hp);
        var hp_effect = FM.LoadHpIncreaseEffect(chess.grid, chess.cur_attack);
    }
}