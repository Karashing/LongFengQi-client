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
public class GongSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public GongSkill0(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 1f;
        name = () => "建造";
        role = () => $"<b>消耗1★</b><br>在1格范围内建造或升级<i>建筑物</i>";
        symbol_sprite_name = () => "gear-hammer";
    }
    private List<TipEffect> tip_effects;
    private List<Vector3Int> able_positions;
    public override bool IsEnable() {
        able_positions = new List<Vector3Int>();
        var adjacents = TileMap6.GetRangeGrids(chess.grid.grid_position, 0, 1);
        foreach (var xpos in adjacents) {
            if (GameInfo.grid_dict.ContainsKey(xpos)) {
                var xgrid = GameInfo.grid_dict[xpos];
                if (xgrid.camp == XCamp.ENEMY || xgrid.camp == XCamp.PUBLIC_ENEMY) continue;
                if (xgrid.state == GridState.HAVING && (xgrid.bind_chess.camp == XCamp.ENEMY || xgrid.bind_chess.camp == XCamp.PUBLIC_ENEMY)) continue;
                if (xgrid.type == GridType.AREA) {
                    if (chess.level >= 0)
                        able_positions.Add(xpos);
                }
                if (xgrid.type == GridType.QIANG) {
                    if (chess.level >= 1)
                        able_positions.Add(xpos);
                }
                if (xgrid.type == GridType.LOU) {
                    if (chess.level >= 2)
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
        if (able_positions.Contains(xpos))
            AddExtraDataSelectPosition(xpos);
    }

    public override void Execute(XExtraData data) {
        chess.energy += 35;
        if (chess.camp == XCamp.SELF) GameInfo.bean -= 1;
        var xgrid = GameInfo.grid_dict[data.select_positions[^1]];

        GridType new_grid_type = GridType.None;
        if (xgrid.type == GridType.AREA) {
            new_grid_type = GridType.QIANG;
        }
        if (xgrid.type == GridType.QIANG) {
            new_grid_type = GridType.LOU;
        }
        if (xgrid.type == GridType.LOU) {
            new_grid_type = GridType.FU;
        }
        GameInfo.RemoveActor(xgrid);
        var new_grid = FM.LoadGrid(new(xgrid.server_id, xgrid.grid_position, new_grid_type, xgrid.level, chess.camp));
        if (xgrid.state == GridState.HAVING) {
            xgrid.bind_chess.ChangeGrid(new_grid);
        }
        xgrid.DestroyGameObject();

        var effect = FM.LoadEffect("gong_skill0_effect");
        effect.transform.position = GM.grid_map.GetCellCenterWorld(new_grid);
        effect.Play();
    }
}