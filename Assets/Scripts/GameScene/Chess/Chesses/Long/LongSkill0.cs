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
public class LongSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public LongSkill0(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 3f;
        name = () => "盘龙耀跃";
        role = () => $"<b>消耗1★</b><br>选择一相邻位置的敌人。<br>对敌方单体造成<color=red><b>{chess.cur_attack + 2 * (chess.level + 1)}</b></color>点伤害，并对其身后扇形区域内的敌方单位，造成<color=red><b>{chess.level + 1}</b></color>点扩散伤害";
        symbol_sprite_name = () => "spiked-dragon-head";
    }
    private List<TipEffect> tip_effects;
    private List<Vector3Int> able_positions;
    public override bool IsEnable() {
        able_positions = new List<Vector3Int>();
        var adjacents = TileMap6.GetAdjacentGrids(chess.grid.grid_position);
        foreach (var xpos in adjacents) {
            if (GameInfo.grid_dict.ContainsKey(xpos)) {
                var xgrid = GameInfo.grid_dict[xpos];
                if (xgrid.CanBeTarget(XTarget.ANY, XCamp.ENEMY, XCamp.PUBLIC_ENEMY)) {
                    able_positions.Add(xpos);
                }
            }
        }
        if (able_positions.Count > 0 && GameInfo.bean >= 1)
            return true;
        else
            return false;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        if (extra_data.select_positions.Count >= 1) {
            return true;
        }
        return false;
    }
    public override void BeSelect() {
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
            AddExtraDataSelectPosition(xpos);
            // var adjacents = TileMap6.GetAdjacentGrids(xpos);
            // foreach (var xxpos in adjacents) {
            //     if (TileMap6.GetDistance(chess.grid.grid_position, xxpos) >= 2) {
            //         if (GameInfo.grid_dict.ContainsKey(xxpos)) {
            //             var xxgrid = GameInfo.grid_dict[xxpos];
            //             if (xxgrid.CanBeTarget(XTarget.ANY, XCamp.ENEMY, XCamp.PUBLIC_ENEMY)) {
            //                 AddExtraDataSelectPosition(xxpos);
            //             }
            //         }
            //     }
            // }
        }
    }

    public override void Execute(XExtraData data) {
        if (chess.camp == XCamp.SELF) GameInfo.bean -= 1;
        // XGrid xgrid = null;
        // var xgrids = new List<XGrid>();
        // for (int i = 0; i < data.select_positions.Count; ++i) {
        //     var xxgrid = GameInfo.grid_dict[data.select_positions[i]];
        //     if (i == 0) {
        //         xgrid = xxgrid;
        //     }
        //     else {
        //         xgrids.Add(xxgrid);
        //     }
        // }
        var xgrid = GameInfo.grid_dict[data.select_positions[^1]];
        var xchess = xgrid.bind_chess;
        var adjacents = TileMap6.GetAdjacentGrids(xgrid.grid_position);
        var xgrids = new List<XGrid>();
        foreach (var xxpos in adjacents) {
            if (TileMap6.GetDistance(chess.grid.grid_position, xxpos) >= 2) {
                if (GameInfo.grid_dict.ContainsKey(xxpos)) {
                    var xxgrid = GameInfo.grid_dict[xxpos];
                    if (xxgrid.CanBeTarget(XTarget.ANY, chess.opposite_camp, XCamp.PUBLIC_ENEMY)) {
                        xgrids.Add(xxgrid);
                    }
                }
            }
        }
        // TODO: 594 071
        var effect = FM.LoadEffect("long_skill0_effect", () => {
            if (xgrid.have_hp) {
                chess.Attack(chess.cur_attack + 2 * (chess.level + 1), xgrid);
            }
            else {
                chess.Attack(chess.cur_attack + 2 * (chess.level + 1), xgrid.bind_chess);
            }
            foreach (var xxgrid in xgrids) {
                if (xxgrid.have_hp) {
                    chess.Attack(chess.level + 1, xxgrid);
                }
                else {
                    chess.Attack(chess.level + 1, xxgrid.bind_chess);
                }
            }
        });
        var st_pos = GM.grid_map.GetCellCenterWorld(chess.grid);
        var ed_pos = GM.grid_map.GetCellCenterWorld(xgrid);
        effect.transform.position = st_pos;
        effect.transform.eulerAngles = MathI.GetRotationFromVector(ed_pos - st_pos);
        effect.Play();

    }
}