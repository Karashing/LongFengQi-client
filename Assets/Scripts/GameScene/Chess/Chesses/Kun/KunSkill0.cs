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
public class KunSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public KunSkill0(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 1f;
        name = () => "化羽垂天,抟风九万";
        role = () => $"<b>消耗1★</b><br>为自己与所有相邻的我方单体附加{(int)(0.5f * chess.max_hp)}点的护盾，持续2回合（当自身行动时，回合数减一）";
        symbol_sprite_name = () => "feathered-wing";
    }
    public override bool IsEnable() {
        if (GameInfo.bean >= 1)
            return true;
        else
            return false;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        return is_confirm;
    }

    public override void Execute(XExtraData data) {
        if (chess.camp == XCamp.SELF) GameInfo.bean -= 1;
        var able_actors = new List<XActor>();
        var adjacents = TileMap6.GetRangeGrids(chess.grid.grid_position, 0, 1);
        foreach (var xpos in adjacents) {
            if (GameInfo.grid_dict.ContainsKey(xpos)) {
                var xgrid = GameInfo.grid_dict[xpos];
                if (xgrid.CanBeTarget(XTarget.CHESS, chess.camp)) {
                    able_actors.Add(xgrid.bind_chess);
                }
            }
        }
        chess.EndBuff<KunShieldBuff>();
        chess.owner_buffs.Add(new KunShieldBuff(chess, able_actors));
        //TODO
        // 540 650 725 774 782 783 804
        var effect = FM.LoadEffect("kun_skill0_effect");
        effect.transform.position = GM.grid_map.GetCellCenterWorld(chess.grid);
        effect.Play();
    }
}