using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class TaBuff2 : XBuff, IAfterAttackBuff, ISpeedBuff {
    public TaBuff2(XActor xowner_actor, List<XActor> xtarget_actors) {
        trigger_type = BuffTriggerType.AFTER_ACT;
        lifetime = new BuffLifetimeTimesLimit(1);
        describe = $"我方每次攻击后，龙凤塔释放追加攻击，造成敌方{30}%最大生命值（最高不超过{6}点）的真实伤害。并使速度提高<color=#fb9725><b>{20}</color></b>点";

        Init(xowner_actor, xtarget_actors);
    }

    public void AfterAttack(XHpActor target) {
        var extra_data = new XExtraData();
        if (target is XChess xchess && xchess.CanBeTarget(XTarget.ANY, owner_actor.opposite_camp, XCamp.PUBLIC_ENEMY)) {
            extra_data.AddSelectPositions(xchess.grid.grid_position);
            var xaction = new XAction(owner_actor, owner_actor.extra_skill, GameInfo.cur_action.round, GameInfo.cur_action.action_id);
            xaction.SetActionName("龙凤之力");
            NM.add_extra_actor_action.Send(new(xaction,
                                                 new List<XExtraData> { extra_data }));
        }
        else if (target is XGrid xgrid && xgrid.CanBeTarget(XTarget.ANY, owner_actor.opposite_camp, XCamp.PUBLIC_ENEMY)) {
            extra_data.AddSelectPositions(xgrid.grid_position);
            var xaction = new XAction(owner_actor, owner_actor.extra_skill, GameInfo.cur_action.round, GameInfo.cur_action.action_id);
            xaction.SetActionName("龙凤之力");
            NM.add_extra_actor_action.Send(new(xaction,
                                                 new List<XExtraData> { extra_data }));
        }
    }

    public float GetDeltaSpeed() {
        return 20f;
    }

    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }
}