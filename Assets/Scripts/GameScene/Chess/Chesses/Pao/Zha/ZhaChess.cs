using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ZhaChess : XChess {
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new ZhaSkill0(this, 0),
        };
    }
    public override void ActInteractStart(List<XSkill> xskills, bool is_auto = false) {
        base.ActInteractStart(xskills, true);
        var skill = skills[0];
        if (skill.IsEnable()) {
            skill.BeSelect();
            skill.ConfirmSkill();
        }
    }
    public override bool CanBeBuff() {
        return false;
    }
    public override bool CanBeBuff<TBuff>() {
        return false;
    }
    public override bool CanBeTarget(XCamp xcamp) {
        if (camp != xcamp) return false;
        if (camp == XCamp.SELF) return false;
        if (hp <= 0) return false;
        return true;
    }
    protected override void BeInjured(XActor source_actor) {
        Kill();
    }
}