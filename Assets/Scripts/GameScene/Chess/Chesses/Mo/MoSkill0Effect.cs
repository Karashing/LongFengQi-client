using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

public class MoSkill0Effect : XEffectAction {
    public XSpineAnim first_anim, second_anim;

    public override void Init(UnityAction call_back) {
        base.Init(call_back);
        first_anim.gameObject.SetActive(false);
        first_anim.on_complete = () => BetweenAnim();
        second_anim.gameObject.SetActive(false);
        second_anim.on_event.Add("hit", () => call_back());
        second_anim.on_complete = () => Destroy(gameObject);
    }

    public override void Play() {
        first_anim.gameObject.SetActive(true);
        first_anim.Play();
    }
    void BetweenAnim() {
        first_anim.gameObject.SetActive(false);
        second_anim.gameObject.SetActive(true);
        second_anim.Play();
    }
}
