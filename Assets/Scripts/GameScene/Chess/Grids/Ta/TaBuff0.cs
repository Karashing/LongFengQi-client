using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class TaBuff0 : XBuff, IAttackBuff, ISpeedBuff {
    public TaBuff0(XActor xowner_actor, List<XActor> xtarget_actors) {
        trigger_type = BuffTriggerType.AFTER_ACT;
        lifetime = new BuffLifetimeTimesLimit(1);
        describe = $"使攻击力提升<color=#fb9725><b>{2}</color></b>点，速度提高<color=#fb9725><b>{10}</color></b>点";

        Init(xowner_actor, xtarget_actors);
    }

    public int GetDeltaAttack() {
        return 2;
    }

    public float GetDeltaSpeed() {
        return 10;
    }

    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }
}