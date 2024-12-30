using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeiSkill0Buff0 : XBuff, IMultiSpeedBuff {
    public WeiSkill0Buff0(XActor xowner_actor, XActor xtarget_actor) {
        trigger_type = BuffTriggerType.AFTER_ACT;
        lifetime = new BuffLifetimeTimesLimit(2);
        describe = $"速度降低20%";

        Init(xowner_actor, xtarget_actor, true);
    }

    public float GetMultiSpeed() {
        return -0.2f;
    }

    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }
}