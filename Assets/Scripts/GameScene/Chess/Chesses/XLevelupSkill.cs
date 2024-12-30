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
public class XLevelupSkill : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public XLevelupSkill(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        interact_mode = 1;
        name = () => "升级";
        role = () => $"消耗<b>准备区</b>的<b>{chess.level + 2}</b>个同类型棋子";
        symbol_sprite_name = () => "bottom-right-3d-arrow";
    }
    private List<XChess> able_chesses;
    public override bool IsEnable() {
        able_chesses = GameInfo.GetSelfPrepareChesss(chess.type);
        if (chess.level < chess.max_level && able_chesses.Count >= chess.level + 2)
            return true;
        else
            return false;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        for (int i = 0; i < Mathf.Min(chess.level + 2, able_chesses.Count); ++i) {
            var xchess = able_chesses[i];
            extra_data.AddSelectPositions(xchess.grid.grid_position);
        }
        return is_confirm;
    }
    public override void Execute(XExtraData data) {
        //TODO
        foreach (var xpos in data.select_positions) {
            var xgrid = GameInfo.grid_dict[xpos];
            if (xgrid.state == GridState.HAVING) {
                var xchess = xgrid.bind_chess;
                xchess.Kill();
            }
        }
        chess.level = Mathf.Min(chess.level + 1, chess.max_level);
        var effect = FM.LoadEffect("chess_level_up_effect");
        effect.transform.position = chess.transform.position;
        effect.Play();
    }
}