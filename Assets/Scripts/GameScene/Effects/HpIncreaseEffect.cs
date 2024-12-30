using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HpIncreaseEffect : BaseBehaviour {
    public TMP_Text text;
    private XActor actor;
    public void Init(XActor xactor, int hp_increase) {
        gameObject.SetActive(true);
        actor = xactor;
        text.text = "+" + hp_increase;
        transform.position = actor.transform.position + Vector3.up * 0.5f;
        if (xactor is XChess xchess) {
            text.color = new Color(0f, 0.9f, 0.375f, 1f);
        }
        else if (xactor is XGrid xgrid) {
            text.color = new Color(0.75f, 0.65f, 0.25f, 1f);
        }
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(transform.position + Vector3.up, 1.5f));
        sequence.Insert(0.5f, DOTween.To(() => text.color,
                        x => text.color = x,
                        new Color(text.color.r, text.color.g, text.color.b, 0.2f), 1f));
        sequence.OnComplete(() => {
            End();
        });
    }

    public void End() {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
