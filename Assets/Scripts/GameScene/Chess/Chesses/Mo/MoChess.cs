using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// 53 424 479 528
public class MoChess : XChess {
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new XRestSkill(this, 0),
            new XMoveSkill(this, 1){
                move_range = 2,
            },
            new XAttackSkill(this, 2),
            new MoSkill0(this, 3),
            // new XReturnSkill(this, 4),
        };
        _extra_skill = new List<XSkill>(){
            new MoSkill0(this, 3000){
                multiplier = 0.5f,
            },
        };
    }
    public override void ActInteractStart(List<XSkill> xskills, bool is_auto = false) {
        foreach (var xskill in xskills) {
            if (xskill.is_auto) {
                is_auto = true;
                base.ActInteractStart(xskills, is_auto);
                xskill.ConfirmSkill();
                return;
            }
        }
        base.ActInteractStart(xskills, is_auto);
    }


}