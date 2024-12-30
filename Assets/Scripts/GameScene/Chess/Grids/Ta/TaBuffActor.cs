using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Networking.Types;

public class TaBuffActor : XActor {
    public TaGrid grid;
    protected override void SetSkills() {
        _skills = new List<XSkill>() {
            new TaBuffActorSkill0(this, 0),
        };
        _extra_skill = new List<XSkill>() {
            new TaBuffActorSkill1(this, 3000),
        };
    }
    public void Init(XActorData data, TaGrid xgrid) {
        grid = xgrid;
        server_id = data.server_id;
        camp = data.local_camp;
        if (grid.phase == 1) {
            word = "龙Buff";
        }
        else if (grid.phase == 3) {
            word = "凤Buff";
        }
        else {
            word = "龙凤Buff";
        }
        GameInfo.AddActor(this);
        EnterActionQueue();
    }
    public override void ActInteractStart(List<XSkill> xskills, bool is_auto = false) {
        base.ActInteractStart(xskills, true);
        var skill = xskills[0];
        if (skill.IsEnable()) {
            skill.ConfirmSkill();
        }
    }
    public override void Kill() {
        base.Kill();
        QuitActionQueue();
        GameInfo.RemoveActor(this);
        DestroyGameObject();
    }
}