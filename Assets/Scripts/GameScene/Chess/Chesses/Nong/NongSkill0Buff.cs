using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class NongSkill0Buff : XBuff, ISpeedBuff {
    public NongSkill0Buff(XActor xowner_actor, XActor xtarget_actor) {
        trigger_type = BuffTriggerType.AFTER_ACT;
        lifetime = new BuffLifetimeTimesLimit(3);
        describe = $"速度提高<color=#fb9725><b>100</color></b>点";

        Init(xowner_actor, xtarget_actor, true);
    }

    public float GetDeltaSpeed() {
        return 100f;
    }

    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }
}