using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KunChess : XChess {
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new XRestSkill(this, 0),
            new KunMoveSkill(this, 1){
                move_range = 2,
            },
            new XAttackSkill(this, 2),
            new KunSkill0(this, 3),
            // new XReturnSkill(this, 4),
        };
    }


}