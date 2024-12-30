using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class XBuff : BaseGame {
    public XActor owner_actor; // Buff发起者
    public List<XActor> target_actors; // 作用对象
    public List<UnityEvent> trigger_events = new List<UnityEvent>();
    public string buff_effect_name = null;
    public bool have_effect {
        get { return buff_effect_name != null; }
    }
    public BuffTriggerType trigger_type = BuffTriggerType.NONE;
    public XBuffLifetime lifetime;
    public string describe;
    protected void Init(XActor xowner_actor, XActor xtarget_actor, bool trigger_by_target_actor = false, string effect_name = null) {
        owner_actor = xowner_actor;
        target_actors = new List<XActor> { xtarget_actor };
        var trigger_actor = xowner_actor;
        if (trigger_by_target_actor) trigger_actor = xtarget_actor;
        buff_effect_name = effect_name;

        foreach (var target_actor in target_actors) {
            Debug.Log(target_actor.word + " add buff: " + describe);
            target_actor.buffs.AddI(this);
        }
        LoadTriggerEvents(trigger_actor);
    }
    protected void Init(XActor xowner_actor, List<XActor> xtarget_actors, string effect_name = null) {
        owner_actor = xowner_actor;
        target_actors = xtarget_actors;
        var trigger_actor = xowner_actor;
        buff_effect_name = effect_name;

        foreach (var target_actor in target_actors) {
            Debug.Log(target_actor.word + " add buff: " + describe);
            target_actor.buffs.AddI(this);
        }
        LoadTriggerEvents(trigger_actor);
    }
    private void LoadTriggerEvents(XActor trigger_actor) {
        trigger_events = new();
        if ((trigger_type & BuffTriggerType.ENTER_ACTION) != 0) {
            trigger_events.Add(trigger_actor.enter_action);
        }
        if ((trigger_type & BuffTriggerType.QUIT_ACTION) != 0) {
            trigger_events.Add(trigger_actor.quit_action);
        }
        if ((trigger_type & BuffTriggerType.BEFORE_ACT) != 0) {
            trigger_events.Add(trigger_actor.start_act);
        }
        if ((trigger_type & BuffTriggerType.AFTER_ACT) != 0) {
            trigger_events.Add(trigger_actor.end_act);
        }
        foreach (UnityEvent trigger_event in trigger_events) {
            trigger_event.AddListener(OnTriggerBuff);
        }
        Debug.Log("trigger_events: " + trigger_events.Count);
    }
    public void AddTargetActor(XActor xtarget_actor) {
        target_actors.Add(xtarget_actor);
        xtarget_actor.buffs.AddI(this);
    }
    public void RemoveTargetActor(XActor xtarget_actor) {
        target_actors.Remove(xtarget_actor);
        xtarget_actor.buffs.RemoveI(this);
        if (target_actors.Count <= 0) End();
    }
    public virtual void OnTriggerBuff() {
        if (lifetime.IsEndAfterTrigger()) {
            End();
        }
    }
    public virtual void End() {
        foreach (UnityEvent trigger_event in trigger_events) {
            trigger_event.RemoveListener(OnTriggerBuff);
        }
        foreach (var target_actor in target_actors) {
            target_actor.buffs.RemoveI(this);
        }
        owner_actor.owner_buffs.Remove(this);
    }
}
