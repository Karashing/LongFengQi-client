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
using Unity.VisualScripting;



[Serializable]
public class XMoveSkill : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    protected XChess chess;
    public int move_range; // [1,)
    public XMoveSkill(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        name = () => "移动&传送";
        role = () => $"选择指定位置，最多移动{move_range}步。<br>可与相邻的我方棋子互换位置。<br>可传送至我方建筑中。";
        symbol_sprite_name = () => "move";
    }
    protected List<TipEffect> tip_effects;
    protected List<Vector3Int> can_move_positions;
    public override bool IsEnable() {
        can_move_positions = new List<Vector3Int>();
        var start_pos = chess.grid.grid_position;
        var range_min = 1;
        var range_max = move_range;
        var queue = new Queue<(Vector3Int, int)>();
        var tags = new HashSet<Vector3Int>();
        queue.Enqueue((start_pos, 0));
        tags.Add(start_pos);
        while (queue.Count > 0) {
            var (pos, distance) = queue.Dequeue();
            if (distance >= range_min && distance <= range_max) {
                var xgrid = GameInfo.grid_dict[pos];
                // if (distance <= 1) {
                //     if (xgrid.CanBeTarget(XTarget.ANY, XCamp.SELF, XCamp.NEUTRAL)) can_move_positions.Add(pos);
                // }
                // else {
                //     if (xgrid.CanBeTarget(XTarget.GRID, XCamp.SELF, XCamp.NEUTRAL)) can_move_positions.Add(pos);
                // }
                // if (xgrid.state == GridState.HAVING || xgrid.CanBeTarget(XTarget.ANY, XCamp.ENEMY, XCamp.PUBLIC_ENEMY)) continue;

                if (xgrid.state == GridState.HAVING) {
                    if (distance <= 1 && xgrid.bind_chess.CanBeTarget(XCamp.SELF)) {
                        can_move_positions.Add(pos);
                    }
                    continue;
                }
                else {
                    if (xgrid.camp == XCamp.ENEMY) {
                        continue;
                    }
                    can_move_positions.Add(pos);
                }
            }
            var adjacents = TileMap6.GetAdjacentGrids(pos);
            foreach (var xpos in adjacents) {
                if (distance + 1 <= range_max && GameInfo.grid_dict.ContainsKey(xpos)) {
                    if (GameInfo.grid_dict[xpos].CanBeTarget()) {
                        if (tags.Add(xpos)) {
                            queue.Enqueue((xpos, distance + 1));
                        }
                    }
                }
            }
        }
        foreach (var xgrid in GameInfo.GetSelfBuildingGrids()) {
            var xpos = xgrid.grid_position;
            if (!can_move_positions.Contains(xpos) && xgrid.state == GridState.EMPTY) {
                can_move_positions.Add(xpos);
            }
        }
        return true;
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
        foreach (var xpos in can_move_positions) {
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
        if (can_move_positions.Contains(xpos))
            AddExtraDataSelectPosition(xpos);
    }

    public override void Execute(XExtraData data) {
        var xpos = data.select_positions[^1];
        var dis = TileMap6.GetDistance(xpos, chess.grid.grid_position);

        chess.energy += Mathf.Min(5 + 5 * dis, 20);
        chess.multi_mil *= Mathf.Min(0.1f + 0.3f * dis, 1);
        var xgrid = GameInfo.grid_dict[xpos];
        // TODO: 塔传送？
        chess.MoveToGrid(xgrid);
    }
}