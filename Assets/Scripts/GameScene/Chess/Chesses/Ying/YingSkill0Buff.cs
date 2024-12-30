using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class YingSkill0Buff : XBuff, IBeAttackBuff {
    public YingSkill0Buff(XActor xowner_actor, XActor xtarget_actor) {
        trigger_type = BuffTriggerType.AFTER_ACT;
        lifetime = new BuffLifetimeTimesLimit(2);
        describe = $"使受到的伤害提升<color=#fb9725><b>1</color></b>点";

        Init(xowner_actor, xtarget_actor, true);
    }

    public int GetDeltaBeAttack() {
        return 1;
    }

    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }
}