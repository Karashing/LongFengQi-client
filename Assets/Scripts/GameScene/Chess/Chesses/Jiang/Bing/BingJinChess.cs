using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class BingJinChess : XChess {
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new XRestSkill(this, 0),
            new XMoveSkill(this, 1){
                move_range = 2,
            },
            new XAttackSkill(this, 2),
        };
    }
    protected override void BeInjured(XActor source_actor) {
        Kill();
    }
}
