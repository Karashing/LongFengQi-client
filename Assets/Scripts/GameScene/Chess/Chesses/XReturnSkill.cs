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
public class XReturnSkill : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public XReturnSkill(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        name = () => "回城";
        role = () => "传送到<b>宫</b>或<b>城</b>中";
        symbol_sprite_name = () => "transportation-rings";
    }
    private Vector3Int able_position;
    public override void Init() {
        base.Init();

    }
    public override bool IsEnable() {
        if (chess.grid.type == GridType.GONG || chess.grid.type == GridType.CHENG) return false;
        foreach (var xgrid in GameInfo.GetGrids(GridType.GONG, chess.camp)) {
            if (xgrid.state == GridState.EMPTY) {
                able_position = xgrid.grid_position;
                return true;
            }
        }
        foreach (var xgrid in GameInfo.GetGrids(GridType.CHENG, chess.camp)) {
            if (xgrid.state == GridState.EMPTY) {
                able_position = xgrid.grid_position;
                return true;
            }
        }
        return false;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        if (GameInfo.grid_dict.ContainsKey(able_position))
            extra_data.AddSelectPositions(GameInfo.grid_dict[able_position].grid_position);
        return is_confirm;
    }
    public override void Execute(XExtraData data) {
        chess.energy += 15;
        var xgrid = GameInfo.grid_dict[data.select_positions[^1]];
        chess.ChangeGrid(xgrid);
        chess.transform.DOMove(GM.grid_map.GetCellCenterWorld(xgrid), 1f).SetEase(Ease.InQuad);
        return;
    }
}