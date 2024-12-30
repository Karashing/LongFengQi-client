using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class KunShieldBuff : XBuff, IShieldBuff {
    public Dictionary<XActor, Shield> shield_dict = new Dictionary<XActor, Shield>();
    public KunShieldBuff(XActor xowner_actor, List<XActor> xtarget_actors) {
        var owner_chess = xowner_actor as XChess;
        trigger_type = BuffTriggerType.BEFORE_ACT;
        lifetime = new BuffLifetimeTimesLimit(2);
        describe = $"获得<color=#fb9725><b>{(int)(0.5f * owner_chess.max_hp)}</color></b>点护盾";

        foreach (var xtarget_actor in xtarget_actors) {
            shield_dict.Add(xtarget_actor, new Shield((int)(0.5f * owner_chess.max_hp)));
        }
        Init(xowner_actor, xtarget_actors);
    }
    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }
    public int BeAttack(int atk, XActor xactor = null) {
        if (shield_dict.ContainsKey(xactor)) {
            var shield = shield_dict[xactor];
            Debug.Log(xactor.word + " shield: " + shield.shield + "/" + shield.max_shield);
            var remain_atk = shield.BeAttack(atk);
            return remain_atk;
        }
        return atk;
    }
    public int GetShield(XActor xactor = null) {
        if (shield_dict.ContainsKey(xactor)) {
            var shield = shield_dict[xactor];
            Debug.Log(xactor.word + " shield: " + shield.shield + "/" + shield.max_shield);
            return shield.shield;
        }
        Debug.Log(xactor.word + " shield: " + "0/0");
        return 0;
    }
}