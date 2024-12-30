using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shi1Chess : XChess {
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new Shi1RestSkill(this, 0),
            new XMoveSkill(this, 1){
                move_range = 2,
            },
            new XAttackSkill(this, 2),
            new Shi1Skill0(this, 3),
            // new XReturnSkill(this, 4),
        };
    }
}
