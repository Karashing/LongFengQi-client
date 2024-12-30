using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TianGrid : XGrid {
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new XRestoreBeanSkill(this, 0)
        };
    }
    public override void UpdateLevel(int delta_level) {
        base.UpdateLevel(delta_level);
        if (level == 0) speed = 80;
        else if (level == 1) speed = 100;
        else if (level == 2) speed = 150;
    }
    public override void BindChess(XChess chess) {
        camp = chess.camp;
        base.BindChess(chess);
        EnterActionQueue();
    }
    public override void UnbindChess() {
        base.UnbindChess();
        camp = XCamp.NEUTRAL;
        QuitActionQueue();
    }
    public override void ActInteractStart(List<XSkill> xskills, bool is_auto = false) {
        base.ActInteractStart(xskills, true);
        var skill = skills[0];
        if (skill.IsEnable()) {
            skill.BeSelect();
            skill.ConfirmSkill();
        }
    }
}