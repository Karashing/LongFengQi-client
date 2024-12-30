using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YaoChess : XChess {
    //closed-barbute
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new XRestSkill(this, 0),
            new XMoveSkill(this, 1){
                move_range = 3,
            },
            new XAttackSkill(this, 2),
            new YaoSkill0(this,3),
            // new XReturnSkill(this, 4),
        };
    }
}
