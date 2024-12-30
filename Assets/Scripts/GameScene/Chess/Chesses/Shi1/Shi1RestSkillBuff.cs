using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class Shi1RestSkillBuff : XBuff, IShieldBuff {
    public Shield shield;
    public Shi1RestSkillBuff(XActor xowner_actor, XActor xtarget_actor) {
        trigger_type = BuffTriggerType.NONE;
        lifetime = new BuffLifetimePermanent();
        describe = $"每层获得<color=#fb9725><b>{xowner_actor.level + 1}</color></b>点护盾";

        shield = new Shield(xowner_actor.level + 1);
        Init(xowner_actor, xtarget_actor);
    }
    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }
    public int BeAttack(int atk, XActor xactor = null) {
        var remain_atk = shield.BeAttack(atk);
        if (shield.shield <= 0) {
            End();
        }
        return remain_atk;
    }
    public int GetShield(XActor xactor = null) {
        return shield.shield;
    }
    public void Execute() {
        shield.MaxShieldChange(owner_actor.level + 1, 2 * (owner_actor.level + 1));
        foreach (var xactor in target_actors) {
            var xchess = xactor as XChess;
            xchess.RefreshBuffEffect();
        }
    }
}