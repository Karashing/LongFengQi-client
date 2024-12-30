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
public class TaBuffActorSkill0 : XSkill {
    // public int skill_id;

    // public XSkillDescribe describe;
    // public XExtraData extra_data;

    // private string actor_server_id;
    private TaBuffActor actor;
    public TaBuffActorSkill0(TaBuffActor xactor, int xskill_id) : base(xactor, xskill_id) {
        actor = xactor;
        name = () => "倒计时";
        role = () => "倒计时结束，力量消失";
        effect_time = () => 0.5f;
        symbol_sprite_name = () => "thor-hammer";
    }
    public override bool IsEnable() {
        return true;
    }
    protected override bool IsInteractEnd(bool is_confirm) {
        return is_confirm;
    }
    public override void Execute(XExtraData data) {
        if (actor.grid.effect != null) {
            if (actor.grid.phase % 2 == 0) {
                actor.grid.effect.Kill();
                actor.grid.effect = null;
            }
        }
        actor.Kill();
    }
}