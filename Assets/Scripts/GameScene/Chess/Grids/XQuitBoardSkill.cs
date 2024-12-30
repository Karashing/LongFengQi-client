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
public class XQuitBoardSkill : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XGrid grid;
    public XQuitBoardSkill(XGrid xgrid, int xskill_id) : base(xgrid, xskill_id) {
        grid = xgrid;
        name = () => "退场";
        role = () => $"将当前格内的棋子回满血并退至准备区";
        symbol_sprite_name = () => "chess-rook";
        EM.actors_change.AddListener(refresh_event.Invoke);
    }
    public override bool IsEnable() {
        if (grid.bind_chess != null) {
            if (GameInfo.self_prepare_chess_num < GameInfo.self_prepare_grid_num)
                return true;
        }
        return false;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        return is_confirm;
    }
    public override void Execute(XExtraData data) {
        XGrid xgrid = null;
        foreach (XGrid grid in GameInfo.GetGrids(GridType.prepare_chess, grid.camp)) {
            if (grid.state == GridState.EMPTY) {
                xgrid = grid;
                break;
            }
        }
        grid.bind_chess.hp = grid.bind_chess.max_hp;
        grid.bind_chess.MoveToGrid(xgrid);
        grid.multi_mil *= 0f;
    }
}