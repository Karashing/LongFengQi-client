using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TipSelect : BaseBehaviour {
    public SpriteRenderer sprite_renderer;
    private XActor actor;
    private List<Sequence> tweens;
    public void Init(XActor xactor) {
        actor = xactor;
        gameObject.SetActive(true);
        if (xactor.camp == XCamp.ENEMY) {
            sprite_renderer.color = FM.GetCampColor(xactor.camp);
        }
        else {
            sprite_renderer.color = Color.white;
        }
        sprite_renderer.color = new Color(sprite_renderer.color.r, sprite_renderer.color.g, sprite_renderer.color.b, 0.1f);
        transform.position = actor.transform.position;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(0.3f, 0.8f).SetEase(Ease.InQuad));
        sequence.Join(DOTween.To(() => sprite_renderer.color,
                                x => sprite_renderer.color = x,
                                new Color(1, 1, 1, 1), 0.8f).SetEase(Ease.InQuad));
        sequence.OnComplete(() => {
            transform.DOLocalRotate(new Vector3(0, 0, 360), 3f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        });
        tweens = new List<Sequence> {
            sequence
        };
    }
    public void End() {
        foreach (var tween in tweens) {
            tween.Kill();
        }
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
