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
public class ZhaSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public ZhaSkill0(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 2f;
        name = () => "定时爆炸";
        role = () => $"对周围所有敌方造成<color=red><b>{chess.cur_attack + chess.level + 1}</b></color>点伤害";
        symbol_sprite_name = () => "unlit-bomb";
    }
    public override bool IsEnable() {
        return true;
    }
    public override void Execute(XExtraData data) {
        var xgrid = chess.grid;
        var adjacents = TileMap6.GetAdjacentGrids(xgrid.grid_position);
        var xgrids = new List<XGrid>();
        foreach (var xpos in adjacents) {
            if (GameInfo.grid_dict.ContainsKey(xpos)) {
                var xxgrid = GameInfo.grid_dict[xpos];
                if (xxgrid.CanBeTarget(XTarget.ANY, chess.opposite_camp, XCamp.PUBLIC_ENEMY)) {
                    xgrids.Add(xxgrid);
                }
            }
        }
        var effect = FM.LoadEffect("zha_skill0_effect", () => {
            foreach (var xxgrid in xgrids) {
                if (xxgrid.have_hp) {
                    chess.Attack(chess.cur_attack + 1, xxgrid);
                }
                else {
                    chess.Attack(chess.cur_attack + 1, xxgrid.bind_chess);
                }
            }
            chess.Kill();
        });
        effect.transform.position = GM.grid_map.GetCellCenterWorld(xgrid);
        effect.Play();
    }
}