using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChengGrid : XGrid {
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new XRecoverSkill(this, 0){
                recover_hp_rate = 0.5f,
            },
            new XQuitBoardSkill(this, 1),
        };
    }
    public override void BindChess(XChess chess) {
        base.BindChess(chess);
        EnterActionQueue();
    }
    public override void UnbindChess() {
        base.UnbindChess();
        QuitActionQueue();
    }
}