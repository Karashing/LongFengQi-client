using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TianActor : XActor {
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new XRestoreBeansSkill(this, 0)
        };
    }
    public void Init(XActorData xactor) { // TODO: 区分我方/敌方
        server_id = xactor.server_id;

        // level = xactor.level;
        camp = xactor.local_camp;
        GameInfo.AddActor(this);
        EnterActionQueue();

    }

    public override void ActInteractStart(List<XSkill> xskills, bool is_auto = false) {
        base.ActInteractStart(xskills);
        var skill = skills[0];
        if (skill.IsEnable()) {
            skill.BeSelect();
            skill.ConfirmSkill();
        }
        else {
            XSkill.ExecuteNoneSkill(server_id, act_round);
        }
    }
}