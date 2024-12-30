using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class NongBigSkillBuff : XBuff, ICoinBeanBuff {
    public NongBigSkillBuff(XActor xowner_actor, List<XActor> xtarget_actors) {
        trigger_type = BuffTriggerType.BEFORE_ACT;
        lifetime = new BuffLifetimeTimesLimit(2);
        describe = $"可以从<i>田</i>中获得额外{5 * (xowner_actor.level + 1)}枚金币";

        Init(xowner_actor, xtarget_actors);
    }

    public int GetDeltaCoin() {
        return 5 * (owner_actor.level + 1);
    }

    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }
}