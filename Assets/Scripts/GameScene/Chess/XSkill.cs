using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEditor;



[Serializable]
public class XSkill : BaseGame {
    public int skill_id;
    public static int GetSkillType(int skill_code) {
        return skill_code / 1000;
    }
    public static int GetSkillId(int skill_code) {
        return skill_code % 1000;
    }

    public int interact_mode = 0;
    public bool is_auto {
        get {
            return GetSkillType(skill_id) == 3;
        }
    }
    public Func<float> effect_time;
    public XSkillDescribe describe;
    public Func<string> name;
    public Func<string> role;
    public Func<string> symbol_sprite_name;
    public Sprite symbol_sprite {
        get {
            return FM.GetIconSprite(symbol_sprite_name());
        }
    }
    public XExtraData extra_data;
    public UnityEvent refresh_event;
    private XActor actor;
    private int act_round;
    private int action_id;

    public XSkill(XActor xactor, int xskill_id) {
        actor = xactor;
        skill_id = xskill_id;
        effect_time = () => 0.5f;
        extra_data = new XExtraData();
        refresh_event = new UnityEvent();
    }
    public virtual void Init() {
        act_round = GameInfo.cur_round;
        action_id = GameInfo.cur_action_id;
    }
    public virtual bool IsEnable() {
        return true;
    }
    public virtual void BeSelect() {
        extra_data = new XExtraData();
    }
    public virtual void CancelSelect() {
    }
    public virtual void Execute(XExtraData data) {

    }
    protected virtual bool IsInteractEnd(bool is_confirm) {
        return is_confirm;
    }

    // public virtual void OnSelectGrid(XGrid xgrid) { }
    public virtual void OnSelectPosition(Vector3Int xpos) {
        extra_data.NewSelectPositions();
    }
    // protected virtual void AddExtraDataSelectGrid(XGrid xgrid) {
    //     extra_data.grid_server_ids.Add(xgrid.server_id);
    //     if (IsInteractEnd(false)) {
    //         ConfirmInteract();
    //     }
    // }
    protected virtual void AddExtraDataSelectActor(XActor xactor) {
        extra_data.actor_server_ids.Add(xactor.server_id);
        if (IsInteractEnd(false)) {
            ConfirmInteract();
        }
    }
    protected virtual void AddExtraDataSelectPosition(Vector3Int xpos) {
        extra_data.AddSelectPositions(xpos);
        if (IsInteractEnd(false)) {
            ConfirmInteract();
        }
    }
    public virtual void ConfirmSkill() {
        if (IsInteractEnd(true))
            ConfirmInteract();
    }
    private void ConfirmInteract() {
        Debug.Log("Skill: " + name());
        NM.actor_interact.Send(new(act_round, action_id, effect_time(), actor.server_id, skill_id, extra_data));
        extra_data = new XExtraData();
    }
    public static void ExecuteNoneSkill(string server_id, int round, int action_id = 0) {
        NM.actor_interact.Send(new(round, action_id, 0.5f, server_id, -1, new()));
    }
}