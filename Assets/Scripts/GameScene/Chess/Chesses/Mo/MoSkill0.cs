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
public class MoSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public float multiplier = 1f;
    public MoSkill0(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 1.6f;
        name = () => "魔斩";
        role = () => $"<b>消耗1★</b><br>选择一相邻位置的敌人。<br>朝该位置发动扇形范围的攻击，对主目标（敌方）造成<color=red><b>{GetNum(0)}</b></color>点伤害，并对其余目标（敌方）造成<color=red><b>{GetNum(1)}</b></color>点扩散伤害。<br>并使自身进入<b>狂魔</b>状态，持续<b>2</b>回合。<br><b>狂魔</b>期间，受到攻击后将自动释放本技能进行反击，造成的伤害为原伤害的50%";
        symbol_sprite_name = () => "warlord-helmet";
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
        if (able_positions.Contains(xpos))
            AddExtraDataSelectPosition(xpos);
    }

    public override void Execute(XExtraData data) {
        if (chess.camp == XCamp.SELF) GameInfo.bean -= 1;
        var xgrid = GameInfo.grid_dict[data.select_positions[^1]];

        var adjacents = TileMap6.GetAdjacentGrids(xgrid.grid_position);
        var xgrids = new List<XGrid>();
        foreach (var xxpos in adjacents) {
            if (TileMap6.GetDistance(chess.grid.grid_position, xxpos) == 1) {
                if (GameInfo.grid_dict.ContainsKey(xxpos)) {
                    var xxgrid = GameInfo.grid_dict[xxpos];
                    if (xxgrid.CanBeTarget(XTarget.ANY, chess.opposite_camp, XCamp.PUBLIC_ENEMY)) {
                        xgrids.Add(xxgrid);
                    }
                }
            }
        }

        chess.EndBuff<MoSkill0Buff>();
        chess.owner_buffs.Add(new MoSkill0Buff(chess, chess));
        var effect = FM.LoadEffect("mo_skill0_effect", () => {
            if (xgrid.have_hp) {
                chess.Attack(GetNum(0), xgrid);
            }
            else {
                chess.Attack(GetNum(0), xgrid.bind_chess);
            }
            foreach (var xxgrid in xgrids) {
                if (xxgrid.have_hp) {
                    chess.Attack(GetNum(1), xxgrid);
                }
                else {
                    chess.Attack(GetNum(1), xxgrid.bind_chess);
                }
            }
        });
        var st_pos = GM.grid_map.GetCellCenterWorld(chess.grid);
        var ed_pos = GM.grid_map.GetCellCenterWorld(xgrid);
        effect.transform.position = st_pos;
        effect.transform.eulerAngles = MathI.GetRotationFromVector(st_pos - ed_pos);
        effect.Play();
    }
    public int GetNum(int id) {
        if (id == 0) {
            return (int)(multiplier * (chess.cur_attack + chess.level + 1));
        }
        else {
            return (int)(multiplier * 0.5f * (chess.cur_attack + chess.level + 1));
        }
    }
}