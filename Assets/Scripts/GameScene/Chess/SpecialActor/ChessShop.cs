using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessShop : XActor {

    public void Init(XActorData xshop) {
        Debug.Log("ChessShop Start Init");
        try {
            server_id = xshop.server_id;
            // level = xshop.level;
            camp = xshop.local_camp;
            NM.build_actor.AddCallback(BuyChess);
            // owner_buffs.Add(new FirstEnterShopBuff(this, new() { this }));
            GameInfo.AddActor(this);
            EnterActionQueue();
            Debug.Log("ChessShop Init Successed");
        }
        catch (Exception e) {
            Debug.Log("ChessShop Exception: " + e);
        }
    }
    public override void ActInteractStart(List<XSkill> xskills, bool is_auto = false) {
        GM.shop_module.Init(this);
        base.ActInteractStart(xskills, true);
    }
    public override void ActInteractEnd() {
        GM.shop_module.End();
        base.ActInteractEnd();
    }


    void BuyChess(ActorData actor_data) {
        if (actor_data.actor_type == ActorType.CHESS) {
            ChessData chess_data = actor_data.LoadData<ChessData>();
            var base_chess = GameData.chess_dict[chess_data.type];
            if (chess_data.local_camp == XCamp.SELF) {
                GameInfo.coin -= base_chess.rarity;
            }
            var xchesses = GameInfo.GetChesss(chess_data.type, chess_data.local_camp);
            if (xchesses.Count > 0) {
                XChess target_chess = null;
                foreach (var xchess in xchesses) {
                    if (xchess.level < xchess.max_level) {
                        target_chess = xchess;
                        break;
                    }
                }
                if (target_chess) {
                    target_chess.MergeChessToLevelUp();
                    return;
                }
            }
            FM.LoadChess(chess_data);
        }
    }
}