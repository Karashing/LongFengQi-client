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
public class WeiSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public WeiSkill0(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 2f;
        name = () => "逐客令";
        role = () => $"<b>消耗1★</b><br>选择范围2格内的一名敌人。<br>对敌方单体造成<color=red><b>{chess.cur_attack}</b></color>点伤害，并附加<b>驱逐</b>效果，持续2回合。<br><b>驱逐</b>效果期间，使目标速度降低20%。如果自己所在地域为<i>我方建筑物</i>，则额外有50%的概率对目标造<b>眩晕</b>效果，持续1回合。";
        symbol_sprite_name = () => "warlock-hood";
    }
    private List<TipEffect> tip_effects;
    private List<Vector3Int> able_positions;
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
        if (able_positions.Count > 0 && GameInfo.bean >= 1)
            return true;
        else
            return false;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        if (extra_data.select_positions.Count >= 1) {
            extra_data.ramdon_nums.Add(UnityEngine.Random.Range(0f, 1f));
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

        var effect = FM.LoadEffect("wei_skill0_effect", () => {
            if (xgrid.have_hp) {
                chess.Attack(chess.cur_attack, xgrid);
            }
            else {
                chess.Attack(chess.cur_attack, xgrid.bind_chess);
                var xchess = xgrid.bind_chess;
                xchess.EndBuff<WeiSkill0Buff0>();
                xchess.owner_buffs.Add(new WeiSkill0Buff0(xchess, xchess));
                if (chess.grid.in_building) {
                    if (data.ramdon_nums[0] <= 0.5f) {
                        xchess.EndBuff<WeiSkill0Buff0>();
                        xchess.owner_buffs.Add(new WeiSkill0Buff1(xchess, xchess));
                    }
                }
            }
        }) as WeiSkill0Effect;
        var st_pos = GM.grid_map.GetCellCenterWorld(chess.grid);
        var ed_pos = GM.grid_map.GetCellCenterWorld(xgrid);
        effect.transform.position = st_pos;
        effect.target_pos = ed_pos;
        effect.Play();
    }
}