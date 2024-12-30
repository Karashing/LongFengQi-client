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
public class XEnterBoardSkill : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XGrid grid;
    public XEnterBoardSkill(XGrid xgrid, int xskill_id) : base(xgrid, xskill_id) {
        grid = xgrid;
        name = () => "上场";
        role = () => $"选择一个准备区的棋子上场<br>棋子将移动至宫或城中<br>场上同时只能存在一枚同类型的棋子<br>当前最多能上场{GameInfo.can_inboard_chess_num}个棋子";
        symbol_sprite_name = () => "chess-rook";
        EM.exp_change.AddListener(refresh_event.Invoke);
        EM.actors_change.AddListener(refresh_event.Invoke);
    }
    private List<TipEffect> tip_effects;
    private List<Vector3Int> able_positions;
    private XGrid target_actor;
    public override bool IsEnable() {
        able_positions = new List<Vector3Int>();
        var xchesses = GameInfo.GetSelfInBoardChesses();
        var xchess_types = new List<ChessType>();
        foreach (var xchess in xchesses) {
            xchess_types.Add(xchess.type);
        }
        foreach (var xgrid in GameInfo.self_prepare_grids) {
            if (xgrid.state == GridState.HAVING) {
                if (!xchess_types.Contains(xgrid.bind_chess.type)) {
                    able_positions.Add(xgrid.grid_position);
                }
            }
        }

        target_actor = null;
        if (grid.state == GridState.EMPTY) target_actor = grid;
        else {
            foreach (var xgrid in GameInfo.GetGrids(GridType.CHENG, grid.camp)) {
                if (xgrid.state == GridState.EMPTY) {
                    target_actor = xgrid;
                    break;
                }
            }
        }

        if (able_positions.Count > 0 && target_actor != null && GameInfo.self_in_board_countable_chess_num < GameInfo.can_inboard_chess_num) {
            return true;
        }
        return false;
    }
    public override void BeSelect() {
        IsEnable();
        base.BeSelect();
        tip_effects = new List<TipEffect>();
        foreach (var xpos in able_positions) {
            tip_effects.Add(FM.LoadTipEffect(xpos));
        }
    }
    public override void CancelSelect() {
        foreach (var tip_effect in tip_effects) {
            tip_effect.End();
        }
    }
    public override void OnSelectPosition(Vector3Int xpos) {
        base.OnSelectPosition(xpos);
        if (able_positions.Contains(xpos)) {
            var xgrid = GameInfo.grid_dict[xpos];
            if (xgrid.bind_chess == null)
                Debug.LogError($"{xgrid.word} xgrid.bind_chess==null");
            else {
                extra_data.actor_server_ids.Add(target_actor.server_id);
                AddExtraDataSelectActor(xgrid.bind_chess);
            }
        }
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        if (extra_data.actor_server_ids.Count >= 1) {
            return true;
        }
        return false;
    }
    public override void Execute(XExtraData data) {
        var xgrid = GameInfo.actor_dict[data.actor_server_ids[^2]] as XGrid;
        var xchess = GameInfo.actor_dict[data.actor_server_ids[^1]] as XChess;
        xchess.MoveToGrid(xgrid);
    }
}