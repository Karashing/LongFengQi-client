using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

public class LongSkill1Effect : XEffectAction {
    public XSpineAnim first_anim, second_anim;

    public override void Init(UnityAction call_back) {
        base.Init(call_back);
        first_anim.gameObject.SetActive(false);
        second_anim.gameObject.SetActive(false);
        first_anim.on_complete = () => BetweenAnim(call_back);
        second_anim.on_complete = () => Destroy(gameObject);
    }
    public override void Play() {
        first_anim.gameObject.SetActive(true);
        first_anim.Play();
    }
    void BetweenAnim(UnityAction xcallback) {
        first_anim.gameObject.SetActive(false);
        second_anim.gameObject.SetActive(true);
        xcallback();
        second_anim.Play();
    }
}
