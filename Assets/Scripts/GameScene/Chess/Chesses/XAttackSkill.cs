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
public class XAttackSkill : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    protected XChess chess;
    public XAttackSkill(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 2f;
        name = () => "普攻";
        role = () => $"选择一相邻位置的敌人。<br>对敌方单体造成<color=red><b>{chess.cur_attack}</b></color>点伤害<br>如果敌方重伤(HP=0)，则移动到该位置";
        symbol_sprite_name = () => "crossed-swords";
    }
    protected List<TipEffect> tip_effects;
    protected List<Vector3Int> able_positions;
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
        if (able_positions.Count > 0)
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
        chess.energy += 25;
        var xpos = data.select_positions[^1];
        var xgrid = GameInfo.grid_dict[xpos];
        // chess.chess_size;
        var start_pos = chess.transform.position;
        var target_pos = xgrid.transform.position;
        var direct = (target_pos - start_pos).normalized;
        var distance = Vector3.Distance(target_pos, start_pos);
        var chess_r = chess.chess_size.x / 2f;

        if (xgrid.have_hp) {
            UnityAction after_callback = () => {
                var cur_xgrid = GameInfo.grid_dict[xpos];
                if (xgrid.hp == 0 && cur_xgrid.state == GridState.EMPTY && xgrid.CanBeTarget()) {
                    chess.ChangeGrid(cur_xgrid);
                    chess.transform.DOMove(target_pos, 1.5f).SetEase(Ease.OutSine);
                }
                else {
                    chess.transform.DOMove(start_pos, 1.5f).SetEase(Ease.OutSine);
                }
            };
            Sequence sequence = DOTween.Sequence();
            sequence.Append(chess.transform.DOMove(start_pos + direct * (distance - chess_r * 2), 0.6f).SetEase(Ease.OutQuart));
            sequence.InsertCallback(0.4f, () => chess.Attack(chess.cur_attack, xgrid, after_callback: after_callback));
        }
        else {
            var xchess = xgrid.bind_chess;
            var xchess_r = xchess.chess_size.x / 2f;
            UnityAction after_callback = () => {
                if (xchess.hp == 0 && xgrid.CanBeTarget()) {
                    chess.ChangeGrid(xgrid);
                    chess.transform.DOMove(target_pos, 1.5f).SetEase(Ease.OutSine);
                }
                else {
                    Sequence seq = DOTween.Sequence();
                    seq.Append(chess.transform.DOMove(start_pos, 1.5f).SetEase(Ease.OutSine));
                    seq.Join(xchess.transform.DOMove(target_pos, 1.5f).SetEase(Ease.OutSine));
                }
            };
            Sequence sequence = DOTween.Sequence();
            sequence.Append(chess.transform.DOMove(start_pos + direct * (distance - xchess_r), 0.7f).SetEase(Ease.OutQuart));
            // sequence.AppendInterval(0.25f);
            sequence.Insert(0.1f, xchess.transform.DOMove(target_pos + direct * xchess_r, 0.6f).SetEase(Ease.OutQuart));
            sequence.InsertCallback(0.5f, () => chess.Attack(chess.cur_attack, xchess, after_callback: after_callback));
        }
    }
}