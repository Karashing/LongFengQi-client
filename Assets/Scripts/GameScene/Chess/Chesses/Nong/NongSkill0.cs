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
public class NongSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public NongSkill0(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 1f;
        name = () => "科学培育";
        role = () => $"<b>消耗1★</b>使所处位置的<i>田</i>速度提高<color=#5cacee><b>100</color></b>点，持续3个回合<br>该效果对同一单位不能叠加";
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
                if (xgrid.state == GridState.HAVING && xgrid.bind_chess.camp == XCamp.ENEMY) continue;
                if (xgrid.type == GridType.TIAN) {
                    able_positions.Add(xpos);
                }

            }
        }
        if (able_positions.Count > 0 && GameInfo.bean >= 1) {
            return true;
        }
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
        // if (chess.grid.type == GridType.AREA) {
        //     if (chess.camp == XCamp.SELF) GameInfo.coin -= 5;
        //     GameInfo.RemoveActor(xgrid);
        //     var new_grid = FM.LoadGrid(new(xgrid.server_id, xgrid.grid_position, GridType.TIAN, 0, XCamp.NEUTRAL));
        //     if (xgrid.state == GridState.HAVING) {
        //         xgrid.bind_chess.ChangeGrid(new_grid);
        //     }
        //     xgrid.DestroyGameObject();
        //     xgrid = new_grid;
        // }
        // else if (chess.grid.type == GridType.TIAN && xgrid.level == 0) {
        //     if (chess.camp == XCamp.SELF) GameInfo.coin -= 10;
        //     xgrid.level += 1;
        // }
        chess.EndBuff<NongSkill0Buff>();
        chess.owner_buffs.Add(new NongSkill0Buff(chess, xgrid));
        var effect = FM.LoadEffect("gong_skill0_effect");
        effect.transform.position = GM.grid_map.GetCellCenterWorld(xgrid);
        effect.Play();
    }
}