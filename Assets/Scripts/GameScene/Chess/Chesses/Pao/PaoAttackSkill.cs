using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ToolI;
using DG.Tweening;
using UnityEngine.Events;
using UnityEditor;
using System.Threading.Tasks;



[Serializable]
public class PaoAttackSkill : XAttackSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    public PaoAttackSkill(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        effect_time = () => 2f;
        role = () => $"选择范围2格内的一名敌人或一处空地。<br>若选择敌人，则消耗一枚<b>炸弹</b>，对敌方造成<color=red><b>{chess.cur_attack}</b></color>点伤害；若选择空地，则向空地发射一枚炸弹，炸弹延时爆炸，对范围1格内的所有敌方造成<color=red><b>{chess.attack + chess.level + 1}</b></color>点伤害";
        symbol_sprite_name = () => "cannon";
    }
    public override bool IsEnable() {
        able_positions = new List<Vector3Int>();
        var adjacents = TileMap6.GetRangeGrids(chess.grid.grid_position, 1, 2);
        foreach (var xpos in adjacents) {
            if (GameInfo.grid_dict.ContainsKey(xpos)) {
                var xgrid = GameInfo.grid_dict[xpos];
                if (xgrid.CanBeTarget(XTarget.ANY, XCamp.ENEMY, XCamp.PUBLIC_ENEMY) || (xgrid.CanBeTarget(XTarget.GRID, XCamp.NEUTRAL) && xgrid.state == GridState.EMPTY)) {
                    able_positions.Add(xpos);
                }
            }
        }
        var buff = chess.GetBuff<PaoSkill0Buff>();
        if (able_positions.Count > 0 && buff != null && buff.shell_num > 0)
            return true;
        else
            return false;
    }

    protected override bool IsInteractEnd(bool is_confirm) {
        if (extra_data.select_positions.Count >= 1) {
            var xgrid = GameInfo.grid_dict[extra_data.select_positions[^1]];
            if (!xgrid.CanBeTarget(XTarget.ANY, chess.opposite_camp, XCamp.PUBLIC_ENEMY)) {
                extra_data.new_actors.Add(new(new ChessData(
                    serverId: Generate.GenerateId(),
                    chessType: ChessType.ZHA,
                    chessLevel: chess.level,
                    xcamp: chess.camp,
                    chessPositionType: ChessPositionType.grid_server_id,
                    positionInfo: xgrid.server_id
                )));
            }

            return true;
        }
        return false;
    }

    public override void Execute(XExtraData data) {
        var xgrid = GameInfo.grid_dict[data.select_positions[^1]];
        chess.GetBuff<PaoSkill0Buff>().SubShell();

        var effect = FM.LoadEffect("ying_skill0_effect", () => {
            if (xgrid.have_hp) {
                chess.Attack(chess.cur_attack, xgrid);
            }
            else {
                if (!xgrid.CanBeTarget(XTarget.ANY, chess.opposite_camp, XCamp.PUBLIC_ENEMY)) {
                    ChessData chess_data = data.new_actors[0].LoadData<ChessData>();
                    var new_chess = FM.LoadChess(chess_data) as ZhaChess;
                }
                else {
                    chess.Attack(chess.cur_attack, xgrid.bind_chess);
                }
            }
        }) as YingSkill0Effect;
        var st_pos = GM.grid_map.GetCellCenterWorld(chess.grid);
        var ed_pos = GM.grid_map.GetCellCenterWorld(xgrid);
        effect.transform.position = st_pos;
        effect.target_pos = ed_pos;
        effect.distance = TileMap6.GetDistance(chess.grid.grid_position, xgrid.grid_position);
        effect.transform.eulerAngles = MathI.GetRotationFromVector(ed_pos - st_pos, 180);
        effect.Play();
    }
}