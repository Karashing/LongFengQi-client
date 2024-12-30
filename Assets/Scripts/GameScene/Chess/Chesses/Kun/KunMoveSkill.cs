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
public class KunMoveSkill : XMoveSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    public KunMoveSkill(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        name = () => "斗转星移";
        role = () => $"选择{move_range}格内的指定位置，无视障碍移动。<br>可与{move_range}格内的我方棋子互换位置。";
        symbol_sprite_name = () => "multi-directions";
    }
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
                if (xgrid.state == GridState.HAVING) {
                    if (distance <= move_range && xgrid.bind_chess.CanBeTarget(XCamp.SELF)) {
                        can_move_positions.Add(pos);
                    }
                }
                else {
                    if (xgrid.camp != XCamp.ENEMY) {
                        can_move_positions.Add(pos);
                    }
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
        return true;
    }

    public override void Execute(XExtraData data) {
        base.Execute(data);
    }
}