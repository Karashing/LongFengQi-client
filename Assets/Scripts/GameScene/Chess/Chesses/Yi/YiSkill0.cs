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
public class YiSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public YiSkill0(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 1f;
        name = () => "医者仁心";
        role = () => $"<b>消耗1★</b><br>选择范围2格内的我方单体。<br>为指定单体回复<color=red><b>{chess.hp}</b></color>点血量，并为指定单体相邻的血量百分比最低的我方单体回复<color=red><b>{(int)(0.5f * chess.hp)}</b></color>点血量";
        symbol_sprite_name = () => "hospital-cross";
    }
    private List<TipEffect> tip_effects;
    private List<Vector3Int> able_positions;
    public override bool IsEnable() {
        able_positions = new List<Vector3Int>();
        var adjacents = TileMap6.GetRangeGrids(chess.grid.grid_position, 1, 2);
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

        var adjacents = TileMap6.GetAdjacentGrids(xgrid.grid_position);
        var xgrids = new List<XGrid>();
        foreach (var xxpos in adjacents) {
            if (GameInfo.grid_dict.ContainsKey(xxpos)) {
                var xxgrid = GameInfo.grid_dict[xxpos];
                if (xxgrid.CanBeTarget(XTarget.CHESS, chess.camp)) {
                    xgrids.Add(xxgrid);
                }
            }
        }
        XChess ychess = null;
        float min_hp_percentage = -1;
        foreach (var xxgrid in xgrids) {
            if (min_hp_percentage < 0) {
                ychess = xxgrid.bind_chess;
                min_hp_percentage = 1.0f * ychess.hp / ychess.max_hp;
            }
            else if (1.0f * xxgrid.bind_chess.hp / xxgrid.bind_chess.max_hp < min_hp_percentage) {
                ychess = xxgrid.bind_chess;
                min_hp_percentage = 1.0f * ychess.hp / ychess.max_hp;
            }
        }

        var seq = DOTween.Sequence();
        seq.AppendCallback(() => {
            var effect = FM.LoadEffect("yi_skill0_effect");
            effect.transform.position = GM.grid_map.GetCellCenterWorld(xgrid);
            effect.Play();
            xchess.BeRestore(chess.hp);
        });
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => {
            if (ychess != null) {
                var effect = FM.LoadEffect("yi_skill0_effect");
                effect.transform.position = GM.grid_map.GetCellCenterWorld(ychess.grid);
                effect.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                effect.Play();
                ychess.BeRestore((int)(0.5f * chess.hp));
            }
        });
    }
}