using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiChess : XChess {
    //closed-barbute
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new XRestSkill(this, 0),
            new XMoveSkill(this, 1){
                move_range = 2,
            },
            new XAttackSkill(this, 2),
            new SiSkill0(this,3),
            // new XReturnSkill(this, 4),
        };
    }
}
