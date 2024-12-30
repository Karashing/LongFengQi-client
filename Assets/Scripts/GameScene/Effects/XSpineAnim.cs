using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class XSpineAnim : BaseGame {
    public Transform transform {
        get {
            return skeleton_animation.transform;
        }
    }
    public GameObject gameObject {
        get {
            return skeleton_animation.gameObject;
        }
    }
    public SkeletonAnimation skeleton_animation;
    public string animation_name = "animation";
    public bool loop = false;
    public UnityAction on_complete;
    public Dictionary<string, UnityAction> on_event = new Dictionary<string, UnityAction>();

    public void Play() {
        var entry = SetAnimation(animation_name, loop);
        entry.Event += OnEvent;
        if (on_complete != null)
            entry.Complete += (TrackEntry entry) => { on_complete(); };
    }

    public void OnEvent(TrackEntry entry, Spine.Event e) {
        if (entry.Animation.Name == animation_name) {
            if (on_event.ContainsKey(e.Data.Name)) {
                on_event[e.Data.Name]();
            }
        }
    }


    protected TrackEntry SetAnimation(string animation_name, bool loop, float time_scale = 1) {
        TrackEntry entry = skeleton_animation.state.SetAnimation(0, animation_name, loop);
        entry.TimeScale = time_scale;
        return entry;
    }
}