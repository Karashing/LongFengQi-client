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
public class XiaAttackSkill : XAttackSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private int mode {
        get { return chess.HaveBuff<XiaSkill0Buff>() ? 1 : 0; }
    }
    public XiaAttackSkill(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        effect_time = () => {
            if (mode == 0) return 2f;
            else return 2f;
        };
        name = () => {
            if (mode == 0) return "普攻";
            else return "摄伏诸恶";
        };
        role = () => {
            if (mode == 0) return $"选择一相邻位置的敌人。<br>对敌方造成<color=red><b>{chess.cur_attack}</b></color>点伤害<br>如果敌方重伤(HP=0)，则移动到该位置";
            else return $"选择一相邻位置释放。<br>朝该方向发动攻击，攻击范围2格。对该位置上的敌方单体造成<color=red><b>{chess.cur_attack + 1}</b></color>点伤害，并对其身后的敌方单体造成<color=red><b>{2 * (chess.level + 1)}</b></color>点扩散伤害<br>如果该位置在攻击之后不存在任何棋子，则移动到该位置";
        };
        symbol_sprite_name = () => {
            if (mode == 0) return "crossed-swords";
            else return "running-ninja";
        };
    }
    public override bool IsEnable() {
        if (mode == 0) {
            return base.IsEnable();
        }

        able_positions = new List<Vector3Int>();
        var adjacents = TileMap6.GetAdjacentGrids(chess.grid.grid_position);
        foreach (var xpos in adjacents) {
            if (GameInfo.grid_dict.ContainsKey(xpos)) {
                var xgrid = GameInfo.grid_dict[xpos];
                if (xgrid.CanBeTarget(XTarget.ANY, XCamp.ENEMY, XCamp.PUBLIC_ENEMY) || (xgrid.CanBeTarget(XTarget.GRID, XCamp.NEUTRAL) && xgrid.state == GridState.EMPTY)) {
                    able_positions.Add(xpos);
                }
            }
        }
        if (able_positions.Count > 0)
            return true;
        else
            return false;
    }

    public override void Execute(XExtraData data) {
        if (mode == 0) {
            base.Execute(data);
            return;
        }
        var xpos = data.select_positions[^1];
        var xgrid = GameInfo.grid_dict[xpos];
        var target_pos = xgrid.transform.position;

        var rays = TileMap6.GetRayGrids(chess.grid.grid_position, xpos, 2, 2);
        var xgrids = new List<XGrid>();
        foreach (var xxpos in rays) {
            if (GameInfo.grid_dict.ContainsKey(xxpos)) {
                var xxgrid = GameInfo.grid_dict[xxpos];
                if (xxgrid.CanBeTarget(XTarget.ANY, chess.opposite_camp, XCamp.PUBLIC_ENEMY)) {
                    xgrids.Add(xxgrid);
                }
            }
        }

        var effect = FM.LoadEffect("xia_attack_skill_effect", () => {
            if (xgrid.have_hp) {
                UnityAction after_callback = () => {
                    var cur_xgrid = GameInfo.grid_dict[xpos];
                    if (xgrid.hp == 0 && cur_xgrid.state == GridState.EMPTY && xgrid.CanBeTarget()) {
                        chess.ChangeGrid(cur_xgrid);
                        chess.transform.DOMove(target_pos, 1f).SetEase(Ease.OutSine);
                    }
                };
                chess.Attack(chess.cur_attack + 1, xgrid, after_callback: after_callback);
            }
            else if (xgrid.state == GridState.HAVING) {
                var xchess = xgrid.bind_chess;
                UnityAction after_callback = () => {
                    if (xchess.hp == 0 && xgrid.CanBeTarget()) {
                        chess.ChangeGrid(xgrid);
                        chess.transform.DOMove(target_pos, 1f).SetEase(Ease.OutSine);
                    }
                };
                chess.Attack(chess.cur_attack + 1, xchess, after_callback: after_callback);
            }
            else if (xgrid.state == GridState.EMPTY) {
                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(0.3f);
                seq.AppendCallback(() => {
                    chess.ChangeGrid(xgrid);
                    chess.transform.DOMove(target_pos, 1f).SetEase(Ease.OutSine);
                });
            }
            foreach (var xxgrid in xgrids) {
                if (xxgrid.have_hp) {
                    chess.Attack(2 * (chess.level + 1), xgrid);
                }
                else {
                    chess.Attack(2 * (chess.level + 1), xgrid.bind_chess);
                }
            }
        });
        var st_pos = GM.grid_map.GetCellCenterWorld(chess.grid);
        var ed_pos = GM.grid_map.GetCellCenterWorld(xgrid);
        effect.transform.position = st_pos;
        effect.transform.eulerAngles = MathI.GetRotationFromVector(st_pos - ed_pos);
        effect.Play();
    }
}