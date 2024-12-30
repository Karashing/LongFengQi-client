using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

public class MoSkill0BuffEffect : XEffectAction {
    public XSpineAnim spine_anim;

    public override void Init(UnityAction call_back) {
        base.Init(call_back);
        spine_anim.gameObject.SetActive(false);
    }
    public override void Play() {
        spine_anim.gameObject.SetActive(true);
        spine_anim.Play();
    }
}
