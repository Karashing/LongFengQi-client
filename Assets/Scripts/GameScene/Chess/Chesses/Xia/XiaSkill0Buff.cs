using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class XiaSkill0Buff : XBuff, ISpeedBuff {
    public XiaSkill0Buff(XActor xowner_actor, List<XActor> xtarget_actors) {
        trigger_type = BuffTriggerType.AFTER_ACT;
        lifetime = new BuffLifetimeTimesLimit(4);
        describe = "进入<b>侠客行</b>状态，并提升<color=#fb9725><b>80</color></b>点速度";
        Init(xowner_actor, xtarget_actors);
    }

    public float GetDeltaSpeed() {
        return 80f;
    }

    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }
}