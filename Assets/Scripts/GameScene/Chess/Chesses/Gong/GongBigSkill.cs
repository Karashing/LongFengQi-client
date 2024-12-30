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
public class GongBigSkill : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public GongBigSkill(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 1f;
        name = () => "工匠精神";
        role = () => $"<b>消耗{chess.max_energy}点能量</b><br>选择指定我方建筑物进行维修，使其恢复其{30 + chess.level * 20}%最大生命值（上限不超过{10 + chess.level * 5}点）的血量</i>";
        symbol_sprite_name = () => "gear-hammer";
    }
    private List<TipEffect> tip_effects;
    private List<Vector3Int> able_positions;
    public override bool IsEnable() {
        able_positions = new List<Vector3Int>();
        foreach (var xgrid in GameInfo.GetSelfBuildingGrids()) {
            if (xgrid.CanBeTarget(XTarget.GRID, XCamp.SELF)) {
                able_positions.Add(xgrid.grid_position);
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
        chess.energy = 0;
        var xgrid = GameInfo.grid_dict[data.select_positions[^1]];

        var delta_hp = (int)Mathf.Min((0.3f + 0.2f * chess.level) * xgrid.max_hp, 10f + 5f * chess.level);
        xgrid.hp = Mathf.Min(xgrid.hp + delta_hp, xgrid.max_hp);
        var hp_effect = FM.LoadHpIncreaseEffect(xgrid, delta_hp);
        chess.energy += 5;
    }
}