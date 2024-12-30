using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class FirstEnterShopBuff : XBuff {
    public FirstEnterShopBuff(XActor xowner_actor, List<XActor> xtarget_actors) {
        trigger_type = BuffTriggerType.ENTER_ACTION;
        lifetime = new BuffLifetimeTimesLimit(1);
        describe = "使自身立即行动";

        Init(xowner_actor, xtarget_actors);
    }
    public override void OnTriggerBuff() {
        foreach (var target_actor in target_actors) {
            target_actor.BeAdvanceAction(1f);
        }
        base.OnTriggerBuff();
    }
}