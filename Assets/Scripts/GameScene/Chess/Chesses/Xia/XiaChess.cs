using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class XiaChess : XChess {
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new XRestSkill(this, 0),
            new XiaMoveSkill(this, 1){
                move_range = 2,
            },
            new XiaAttackSkill(this, 2),
            new XiaSkill0(this, 3),
            // new XReturnSkill(this, 4),
        };
    }


}