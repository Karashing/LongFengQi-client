using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class TaBuff1 : XBuff, IAttachAttackBuff {
    public TaBuff1(XActor xowner_actor, List<XActor> xtarget_actors) {
        trigger_type = BuffTriggerType.AFTER_ACT;
        lifetime = new BuffLifetimeTimesLimit(1);
        describe = $"每次攻击附带敌方{20}%最大生命值（最高不超过{4}点）的伤害";

        Init(xowner_actor, xtarget_actors);
    }

    public int GetDeltaAttack(XHpActor target) {
        return Mathf.Min((int)(0.2f * target.max_hp), 4);
    }

    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }
}