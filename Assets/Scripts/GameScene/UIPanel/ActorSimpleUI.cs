using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ActorSimpleUI : BaseBehaviour {
    public RectTransform rect_trans { get { return GetComponent<RectTransform>(); } }
    public Image actor_work_img;
    public TMP_Text actor_word_text;
    public XActor actor;
    private float _multi_font_size = 1f;
    public float multi_font_size {
        get { return _multi_font_size; }
        set {
            actor_word_text.fontSize *= value / _multi_font_size;
            _multi_font_size = value;
        }
    }
    public virtual void Init(XActor xactor) {
        actor = xactor;
        _multi_font_size = 1f;
        actor_work_img.sprite = xactor.work_sprite;
        actor_word_text.text = xactor.word;
        actor_word_text.color = FM.GetCampColor(xactor.camp);
    }

    public Sequence GetEnterTween(float duration = 0.4f) {
        actor_word_text.alpha = 0f;
        actor_work_img.color = new(1f, 1f, 1f, 0f);
        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => actor_word_text.alpha,
                              (float a) => { actor_word_text.alpha = a; },
                              1f, 0.5f)
        );
        seq.Join(actor_work_img.DOColor(Color.white, duration));
        return seq;
    }
    public Sequence GetExitTween(float duration = 0.2f) {
        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => actor_word_text.alpha,
                              (float a) => { actor_word_text.alpha = a; },
                              0f, 0.5f));
        seq.Join(actor_work_img.DOColor(new(1f, 1f, 1f, 0f), duration));
        return seq;
    }
}