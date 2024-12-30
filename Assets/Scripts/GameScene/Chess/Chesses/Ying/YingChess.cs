using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class YingChess : XChess {
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new XRestSkill(this, 0),
            new XMoveSkill(this, 1){
                move_range = 2,
            },
            new YingAttackSkill(this, 2),
            new YingSkill0(this, 3),
            // new XReturnSkill(this, 4),
        };
    }
}