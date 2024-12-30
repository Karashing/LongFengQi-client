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
public class JiangSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public JiangSkill0(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 1f;
        name = () => "全军列阵";
        role = () => $"<b>消耗1★</b><br>选择一相邻空位置释放。<br>随机召唤一种类型的【兵】，【兵】分为2种，<b>近战兵</b>和<b>远程兵</b><br>场上最多同时存在一近战兵和一远程兵。当自身重伤时，属于自己的兵会直接阵亡";
        symbol_sprite_name = () => "closed-barbute";
    }
    private List<TipEffect> tip_effects;
    private List<Vector3Int> able_positions;
    private List<XChess> summon_chesses;
    public override bool IsEnable() {
        summon_chesses = new List<XChess>();
        able_positions = new List<Vector3Int>();
        var adjacents = TileMap6.GetAdjacentGrids(chess.grid.grid_position);
        foreach (var xpos in adjacents) {
            if (GameInfo.grid_dict.ContainsKey(xpos)) {
                var xgrid = GameInfo.grid_dict[xpos];
                if (xgrid.CanSummon(chess)) {
                    able_positions.Add(xpos);
                }
            }
        }
        var xbuff = chess.GetBuff<JiangSkill0Buff>();
        if (xbuff != null) {
            foreach (var target_actor in xbuff.target_actors) {
                summon_chesses.Add(target_actor as XChess);
            }
        }
        Debug.Log(summon_chesses.Count);
        if (able_positions.Count > 0 && GameInfo.bean >= 1 && summon_chesses.Count < 2)
            return true;
        else
            return false;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        if (extra_data.select_positions.Count >= 1) {
            var xgrid = GameInfo.grid_dict[extra_data.select_positions[^1]];
            ChessType new_actor_type;
            Debug.Log(summon_chesses.Count);
            if (summon_chesses.Count == 0) {
                var new_actor_id = RandomI.Range(0, 2);
                if (new_actor_id == 0) new_actor_type = ChessType.BING_JIN;
                else new_actor_type = ChessType.BING_YUAN;
            }
            else {
                if (summon_chesses[0].type == ChessType.BING_YUAN) new_actor_type = ChessType.BING_JIN;
                else new_actor_type = ChessType.BING_YUAN;
            }
            Debug.Log(new_actor_type);
            extra_data.new_actors.Add(new(new ChessData(
                serverId: Generate.GenerateId(),
                chessType: new_actor_type,
                chessLevel: chess.level,
                xcamp: chess.camp,
                chessPositionType: ChessPositionType.grid_server_id,
                positionInfo: xgrid.server_id
            )));

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
        ChessData chess_data = data.new_actors[0].LoadData<ChessData>();

        var effect = FM.LoadEffect("jiang_skill0_effect", () => {
            var new_chess = FM.LoadChess(chess_data);
            // Buff
            var xbuff = chess.GetBuff<JiangSkill0Buff>();
            if (xbuff != null) {
                xbuff.AddTargetActor(new_chess);
            }
            else {
                chess.owner_buffs.Add(new JiangSkill0Buff(chess, new_chess));
            }
        });
        effect.transform.position = GM.grid_map.GetCellCenterWorld(xgrid);
        effect.Play();
    }
}