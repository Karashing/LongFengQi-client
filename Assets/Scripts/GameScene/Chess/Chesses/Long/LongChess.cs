using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LongChess : XChess {
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new XRestSkill(this, 0),
            new XMoveSkill(this, 1){
                move_range = 2,
            },
            new XAttackSkill(this, 2),
            new LongSkill0(this, 3),
            new LongSkill1(this, 4),
            // new XReturnSkill(this, 5),
        };
    }


}