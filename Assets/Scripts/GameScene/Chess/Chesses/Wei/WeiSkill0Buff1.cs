using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeiSkill0Buff1 : XBuff, IActionBuff {
    public WeiSkill0Buff1(XActor xowner_actor, XActor xtarget_actor) {
        trigger_type = BuffTriggerType.AFTER_ACT;
        lifetime = new BuffLifetimeTimesLimit(1);
        describe = $"使目标无法行动";

        Init(xowner_actor, xtarget_actor, true, "vertigo_buff_effect");
    }

    public bool CanAction() {
        return false;
    }

    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }
}