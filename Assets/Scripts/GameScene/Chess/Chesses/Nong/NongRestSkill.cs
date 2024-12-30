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
public class NongRestSkill : XRestSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private int mode;
    private XChess chess;
    public NongRestSkill(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        name = () => {
            if (mode == 0) return "休息";
            else return "培育";
        };
        role = () => {
            if (mode == 0) return "原地回复<color=red><b>1</b></color>点血量";
            else return $"使所处位置的<i>田</i>行动提前50%";
        };
        symbol_sprite_name = () => "night-sleep";
    }
    public override void Init() {
        base.Init();
        mode = 0;
        var xgrid = chess.grid;
        if (xgrid.type == GridType.TIAN) {
            mode = 1;
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
        chess.grid.BeAdvanceAction(0.5f);
    }
}