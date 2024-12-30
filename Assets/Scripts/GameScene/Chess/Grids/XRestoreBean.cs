using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using UnityEditor;



[Serializable]
public class XRestoreBeanSkill : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XGrid grid;
    public XRestoreBeanSkill(XGrid xgrid, int xskill_id) : base(xgrid, xskill_id) {
        grid = xgrid;
        name = () => "生产";
        role = () => "生产一定量的资源，如果在自身回合内存在棋子，则该棋子一方获得一定量的资源";
        symbol_sprite_name = () => "maze-cornea";
    }
    public override bool IsEnable() {
        if (grid.state == GridState.HAVING && grid.bind_chess.camp == XCamp.SELF) return true;
        else return false;
    }
    public override void Execute(XExtraData data) {
        if (grid.state == GridState.HAVING && grid.bind_chess.camp == XCamp.SELF) {
            GameInfo.bean = Mathf.Min(GameInfo.bean + 1, GameInfo.max_bean);
            var delta_coin = 0;
            foreach (var xbuff in grid.bind_chess.GetBuffs<ICoinBeanBuff>()) {
                delta_coin += xbuff.GetDeltaCoin();
            }
            GameInfo.coin += delta_coin;
        }
    }
}