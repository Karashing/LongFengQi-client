using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using ToolI;
using UnityEngine;

public class MoSkill0Buff : XBuff, ICounterAttackBuff {
    public MoSkill0Buff(XActor xowner_actor, XActor xtarget_actors) {
        trigger_type = BuffTriggerType.BEFORE_ACT;
        lifetime = new BuffLifetimeTimesLimit(2);
        describe = "被近战攻击时进行反击";

        Init(xowner_actor, xtarget_actors, effect_name: "mo_skill0_buff_effect");
    }
    public int GetCounterAttack(int atk, XActor from_actor = null, XActor to_actor = null) {
        Debug.Log($"GetCounterAttack start");
        if (from_actor != null) {
            XGrid xgrid = null;
            if (from_actor is XChess from_chess) {
                xgrid = from_chess.grid;
            }
            else if (from_actor is XGrid from_grid) {
                xgrid = from_grid;
            }

            var distance = TileMap6.GetDistance(xgrid.grid_position, (owner_actor as XChess).grid.grid_position);
            Debug.Log($"GetCounterAttack: {distance}");
            if (xgrid != null && distance == 1) {
                var extra_data = new XExtraData();
                extra_data.AddSelectPositions(xgrid.grid_position);
                NM.add_extra_actor_action.Send(new(new(owner_actor, owner_actor.extra_skill, GameInfo.cur_action.round, GameInfo.cur_action.action_id),
                                                     new List<XExtraData> { extra_data }));
            }
        }
        return 0;
    }

    public override void OnTriggerBuff() {
        base.OnTriggerBuff();
    }

}