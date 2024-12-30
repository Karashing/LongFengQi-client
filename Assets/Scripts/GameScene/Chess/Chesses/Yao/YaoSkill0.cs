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
public class YaoSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public YaoSkill0(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 2f;
        name = () => "百妖迷思";
        role = () => $"<b>消耗1★</b><br>选择一相邻位置的敌人(无法选定建筑)。<br>对敌方单体造成<color=red><b>{chess.cur_attack + 1}</b></color>点伤害<br>，如果敌方重伤(HP=0)，则自身恢复1点生命值";
        symbol_sprite_name = () => "gooey-daemon";
    }
    private List<TipEffect> tip_effects;
    private List<Vector3Int> able_positions;
    public override bool IsEnable() {
        able_positions = new List<Vector3Int>();
        var adjacents = TileMap6.GetAdjacentGrids(chess.grid.grid_position);
        foreach (var xpos in adjacents) {
            if (GameInfo.grid_dict.ContainsKey(xpos)) {
                var xgrid = GameInfo.grid_dict[xpos];
                if (xgrid.CanBeTarget(XTarget.CHESS, XCamp.ENEMY, XCamp.PUBLIC_ENEMY)) {
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
        var target_pos = xchess.transform.position;

        var effect = FM.LoadEffect("yao_skill0_effect", () => {
            chess.Attack(chess.cur_attack + 1, xchess, after_callback: () => {
                if (xchess.hp == 0) {
                    chess.BeRestore(1);
                }
            });
        });
        effect.transform.position = target_pos;
        effect.Play();
    }
}