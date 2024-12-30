using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class JiangSkill0Buff : XBuff {
    public JiangSkill0Buff(XActor xowner_actor, XActor xtarget_actor) {
        trigger_type = BuffTriggerType.QUIT_ACTION;
        lifetime = new BuffLifetimeTimesLimit(1);
        describe = $"由【将】召唤而来";

        Init(xowner_actor, xtarget_actor);
    }

    public override void OnTriggerBuff() {
        var tmp_actors = new List<XActor>();
        foreach (var xactor in target_actors) {
            tmp_actors.Add(xactor);
        }
        foreach (var target_actor in tmp_actors) {
            var target_chess = target_actor as XChess;
            target_chess.Kill();
        }
        base.OnTriggerBuff();
    }
}