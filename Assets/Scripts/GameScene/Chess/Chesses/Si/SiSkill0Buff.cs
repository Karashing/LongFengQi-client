using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class SiSkill0Buff : XBuff, IAttackBuff {
    public SiSkill0Buff(XActor xowner_actor, XActor xtarget_actor) {
        trigger_type = BuffTriggerType.AFTER_ACT;
        lifetime = new BuffLifetimeTimesLimit(1);
        describe = $"使攻击力提高<color=#fb9725><b>{(int)(1.5f * (xowner_actor as XChess).attack)}</color></b>点";

        Init(xowner_actor, xtarget_actor, true);
    }

    public int GetDeltaAttack() {
        return (int)(1.5f * (owner_actor as XChess).attack);
    }

    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }
}