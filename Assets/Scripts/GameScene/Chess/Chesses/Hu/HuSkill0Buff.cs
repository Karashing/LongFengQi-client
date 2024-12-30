using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class HuSkill0Buff : XBuff, ISpeedBuff, IAttackBuff {
    public HuSkill0Buff(XActor xowner_actor, XActor xtarget_actor) {
        trigger_type = BuffTriggerType.AFTER_ACT;
        lifetime = new BuffLifetimeTimesLimit(3);
        describe = $"使攻击力提高<color=#fb9725><b>{(xowner_actor as XChess).attack}</color></b>点，速度提高<color=#fb9725><b>{(int)((xowner_actor as XChess).speed * 0.25f)}</color></b>点";

        Init(xowner_actor, xtarget_actor, true);
    }

    public int GetDeltaAttack() {
        return (owner_actor as XChess).attack;
    }

    public float GetDeltaSpeed() {
        return (owner_actor as XChess).speed * 0.25f;
    }

    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }
}