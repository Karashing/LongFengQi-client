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
public class YingAttackSkill : XAttackSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    public YingAttackSkill(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        effect_time = () => 2f;
        role = () => $"选择范围2格内的一名敌人。<br>对敌方造成<color=red><b>{chess.cur_attack}</b></color>点伤害";
        symbol_sprite_name = () => "pocket-bow";
    }
    public override bool IsEnable() {
        able_positions = new List<Vector3Int>();
        var adjacents = TileMap6.GetRangeGrids(chess.grid.grid_position, 1, 2);
        foreach (var xpos in adjacents) {
            if (GameInfo.grid_dict.ContainsKey(xpos)) {
                var xgrid = GameInfo.grid_dict[xpos];
                if (xgrid.CanBeTarget(XTarget.ANY, XCamp.ENEMY, XCamp.PUBLIC_ENEMY)) {
                    able_positions.Add(xpos);
                }
            }
        }
        if (able_positions.Count > 0)
            return true;
        else
            return false;
    }

    public override void Execute(XExtraData data) {
        var xgrid = GameInfo.grid_dict[data.select_positions[^1]];

        var effect = FM.LoadEffect("ying_skill0_effect", () => {
            if (xgrid.have_hp) {
                chess.Attack(chess.cur_attack, xgrid);
            }
            else {
                chess.Attack(chess.cur_attack, xgrid.bind_chess);
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