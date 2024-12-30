using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

public class TaBuffActorSkill1Effect : XEffectAction {
    public XSpineAnim first_anim, second_anim;
    public Tween tween;
    public Vector3 target_pos;
    public int distance;

    public override void Init(UnityAction call_back) {
        base.Init(call_back);
        first_anim.gameObject.SetActive(false);
        second_anim.gameObject.SetActive(false);
        second_anim.on_complete = () => Destroy(gameObject);
    }

    public override void Play() {
        first_anim.gameObject.SetActive(true);
        tween = transform.DOMove(target_pos, distance * 0.1f + 0.3f).SetEase(Ease.InQuad).OnComplete(
            () => BetweenAnim(callback)
        );
        first_anim.Play();
    }
    void BetweenAnim(UnityAction xcallback) {
        first_anim.gameObject.SetActive(false);
        second_anim.gameObject.SetActive(true);
        xcallback();
        second_anim.Play();
    }
}
