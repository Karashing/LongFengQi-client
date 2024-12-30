using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class PaoSkill0Buff : XBuff {
    public int shell_num;
    public PaoSkill0Buff(XActor xowner_actor, XActor xtarget_actors) {
        trigger_type = BuffTriggerType.NONE;
        lifetime = new BuffLifetimePermanent();
        describe = "获得1枚炸弹，最多叠加3枚";

        shell_num = 1;
        Init(xowner_actor, xtarget_actors);
    }

    public void AddShell(int delta_num = 1) {
        shell_num += delta_num;
    }
    public void SubShell(int delta_num = 1) {
        shell_num -= delta_num;
    }

    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }
}