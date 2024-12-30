using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using System.Threading;



public class ActionQueue : BaseBehaviour {
    // private List<XActor> actors = new List<XActor>();
    // private List<(XAction action, ExtraActionCard card)> extra_action_list = new List<(XAction action, ExtraActionCard card)>();
    // private List<(XAction action, ActionCard card)> round_list = new List<(XAction action, ActionCard card)>();
    // private List<(XAction action, ExtraActionCard card)> action_list = new List<(XAction action, ExtraActionCard card)>();
    private List<XAction> round_list = new List<XAction>();
    private List<XAction> action_list = new List<XAction>();
    public RectTransform rect_trans;
    public Vector2 card_size;
    public Vector2 spacing;
    private void Awake() {
        EM.actor_mil_change.AddListener(ActorMilChange);
    }
    private void Update() {
        foreach (var xaction in round_list) {
            var card = xaction.card as ActionCard;
            card.SetMilText(xaction.actor.act_time - GameInfo.cur_act_time);
        }
    }
    public void AddActor(XActor actor) {
        round_list.Add(actor.default_action);
        ReSortPosition();
    }
    public void RemoveActor(XActor actor) {
        for (int i = round_list.Count - 1; i >= 0; --i) {
            if (round_list[i].actor == actor) round_list.RemoveAt(i);
        }
        RemoveAction(actor);
        ReSortPosition();
    }
    public void ReSortPosition(bool need_move = true) {
        round_list.Sort((x, y) => {
            if (x.actor.act_time == y.actor.act_time) {
                return long.Parse(x.actor.server_id).CompareTo(long.Parse(y.actor.server_id));
            }
            return x.actor.act_time.CompareTo(y.actor.act_time);
        });
        if (need_move) {
            rect_trans.sizeDelta = new Vector2(rect_trans.sizeDelta.x, (card_size.y + spacing.y) * round_list.Count);
            for (int i = 0; i < round_list.Count; ++i) {
                var card = round_list[i].card as ActionCard;
                Vector2 target_pos = Vector2.zero;
                target_pos.y = -(spacing.y * (i + 1) + card_size.y * (i * 0.9f + 1));
                card.MoveActionCard(target_pos);
                card.SetActing(i + 1, 0);
            }
        }
    }

    public void AddAction(XAction xaction) {
        if (xaction.card == null) xaction.card = FM.LoadExtraActionCard(xaction);
        action_list.Add(xaction);
        ReSortExtraAction();
        LogActionQueue("[After AddAction]\n");
    }
    public void RemoveAction(XActor actor) {
        for (int i = action_list.Count - 1; i >= 0; --i) {
            if (action_list[i].actor == actor) {
                action_list[i].card?.End();
                if (i > 0)
                    action_list.RemoveAt(i);
            }
        }
        ReSortExtraAction();
    }
    public void RemoveAction(int id = 0) {
        if (0 <= id && id < action_list.Count) {
            if (action_list[id].card is ActionCard) {
                if (action_list[id].card != null && !action_list[id].card.is_end) {
                    round_list.Add(action_list[0]);
                    ReSortPosition();
                }
            }
            else {
                action_list[id].card.End();
            }
            action_list.RemoveAt(id);
            ReSortExtraAction();
        }
    }
    public void RemoveAllAction() {
        foreach (var xaction in action_list) {
            xaction.card.End();
        }
        action_list.Clear();
    }
    public void ReSortExtraAction() {
        action_list.Sort((x, y) => {
            return x.action_id.CompareTo(y.action_id);
        });
        for (int i = 0; i < action_list.Count; ++i) {
            var card = action_list[i].card;
            Vector2 target_pos;
            if (i == 0) {
                target_pos = new Vector2(0f, 0f);
            }
            else {
                target_pos = new Vector2(200f + 82f * (i - 1), 0f);
            }
            card.MoveActionCard(target_pos);
            card.SetActing(0, i);
        }
    }
    public XAction NextAction() {
        LogActionQueue("[Before NextAction]\n");
        RemoveAction(0);
        if (action_list.Count <= 0) {
            NextRound();
        }
        var xaction = action_list[0];
        xaction.round = GameInfo.cur_round;
        xaction.action_id = GameInfo.cur_action_id;
        return xaction;
    }
    public void NextRound() {
        RemoveAllAction();
        ReSortPosition(false);
        if (round_list.Count > 0) {
            var xaction = round_list[0];
            round_list.RemoveAt(0);
            AddAction(xaction);
            ReSortPosition();
        }
    }
    public void LogActionQueue(string pre_text = "") {
        var logtext = pre_text;
        foreach (var xaction in action_list) {
            logtext += xaction.GetActionStr() + "\n";
        }
        Debug.Log(logtext);
    }



    void ActorMilChange(XActor actor) {
        ReSortPosition();
    }
}