using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GongChess : XChess {
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new GongRestSkill(this, 0),
            new XMoveSkill(this, 1){
                move_range = 2,
            },
            new XAttackSkill(this, 2),
            new GongSkill0(this, 3),
            // new XReturnSkill(this, 4),
        };
        _big_skill = new List<XSkill>(){
            new GongBigSkill(this, 2000),
        };
    }
}
