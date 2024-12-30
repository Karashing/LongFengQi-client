using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

public class ShiSkill0Effect : XEffectAction {
    public XSpineAnim spine_anim;
    public int hit_times;

    public override void Init(UnityAction<int> call_back) {
        base.Init(call_back);
        spine_anim.gameObject.SetActive(false);
        spine_anim.on_event.Add("hit", () => {
            call_back(hit_times);
            hit_times++;
        });
        spine_anim.on_complete = () => Destroy(gameObject);
    }
    public override void Play() {
        spine_anim.gameObject.SetActive(true);
        spine_anim.Play();
    }
}
