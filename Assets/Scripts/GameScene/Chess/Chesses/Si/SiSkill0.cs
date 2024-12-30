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
public class SiSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public SiSkill0(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 1f;
        name = () => "先发制人";
        role = () => $"<b>消耗1★</b><br>选择范围3格内的一名我方单位。<br>使我方单体立即行动，并提升<color=red><b>{(int)(1.5f * chess.attack)}</b></color>点攻击力";
        symbol_sprite_name = () => "caesar";
    }
    private List<TipEffect> tip_effects;
    private List<Vector3Int> able_positions;
    public override bool IsEnable() {
        able_positions = new List<Vector3Int>();
        var adjacents = TileMap6.GetRangeGrids(chess.grid.grid_position, 1, 3);
        foreach (var xpos in adjacents) {
            if (GameInfo.grid_dict.ContainsKey(xpos)) {
                var xgrid = GameInfo.grid_dict[xpos];
                if (xgrid.CanBeTarget(XTarget.CHESS, XCamp.SELF)) {
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
        if (chess.camp == XCamp.SELF) GameInfo.bean -= 1;
        var xgrid = GameInfo.grid_dict[data.select_positions[^1]];
        var xchess = xgrid.bind_chess;

        xchess.BeAdvanceAction(1);
        chess.EndBuff<SiSkill0Buff>();
        chess.owner_buffs.Add(new SiSkill0Buff(chess, xchess));
        var effect = FM.LoadEffect("si_skill0_effect");
        effect.transform.position = GM.grid_map.GetCellCenterWorld(xgrid);
        effect.Play();
    }
}