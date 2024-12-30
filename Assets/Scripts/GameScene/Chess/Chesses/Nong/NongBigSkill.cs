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
public class NongBigSkill : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private XChess chess;
    public NongBigSkill(XChess xchess, int xskill_id) : base(xchess, xskill_id) {
        chess = xchess;
        effect_time = () => 1f;
        name = () => "隐藏财富";
        role = () => $"<b>消耗{chess.max_energy}点能量</b><br>立刻回复{chess.level + 2}点能量。";
        symbol_sprite_name = () => "gear-hammer";
    }
    public override bool IsEnable() {
        return true;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        return base.IsInteractEnd(is_confirm);
    }

    public override void Execute(XExtraData data) {
        chess.energy = 0;
        if (chess.camp == XCamp.SELF) {
            GameInfo.bean = Mathf.Min(GameInfo.max_bean, GameInfo.bean + chess.level + 2);
        }
        chess.energy += 5;
    }
}