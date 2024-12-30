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
public class XiangSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public XiangSkill0(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 2f;
        name = () => "天地法相";
        role = () => $"<b>消耗1★</b><br>选择方向释放。<br>对指定方向路径上的所有敌方造成<color=red><b>{(int)(1.5f * chess.cur_attack)}</b></color>点伤害";
        symbol_sprite_name = () => "book-cover";
    }
    private List<TipEffect> tip_effects;
    private List<Vector3Int> able_positions;
    public override bool IsEnable() {
        able_positions = new List<Vector3Int>();
        var adjacents = TileMap6.GetRangeGrids(chess.grid.grid_position, 1, 1);
        foreach (var xpos in adjacents) {
            able_positions.Add(xpos);
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
        var xpos = data.select_positions[^1];
        var rays = TileMap6.GetRayGrids(chess.grid.grid_position, xpos, 1);
        var xgrids = new List<XGrid>();
        foreach (var xxpos in rays) {
            if (GameInfo.grid_dict.ContainsKey(xxpos)) {
                var xxgrid = GameInfo.grid_dict[xxpos];
                if (xxgrid.CanBeTarget(XTarget.ANY, chess.opposite_camp, XCamp.PUBLIC_ENEMY)) {
                    xgrids.Add(xxgrid);
                    Debug.Log(xxpos);
                }
            }
        }

        var effect = FM.LoadEffect("xiang_skill0_effect", () => {
            foreach (var xxgrid in xgrids) {
                if (xxgrid.have_hp) {
                    chess.Attack((int)(1.5f * chess.cur_attack), xxgrid);
                }
                else {
                    chess.Attack((int)(1.5f * chess.cur_attack), xxgrid.bind_chess);
                }
            }
        });
        var st_pos = GM.grid_map.GetCellCenterWorld(chess.grid);
        var ed_pos = GM.grid_map.GetCellCenterWorld(xpos);
        effect.transform.position = st_pos;
        effect.transform.eulerAngles = MathI.GetRotationFromVector(ed_pos - st_pos, 180);
        effect.Play();
    }
}