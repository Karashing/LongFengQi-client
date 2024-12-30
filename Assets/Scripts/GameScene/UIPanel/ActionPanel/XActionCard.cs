using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ToolI;
using TMPro;
using System;

public class XActionCard : BaseBehaviour {
    public RectTransform rect_trans;
    public TMP_Text actor_word_text;
    public Image bg_img, work_img;
    public XAction action;
    protected Tween moving_tween;
    public bool is_end = false;
    public virtual void Init(XAction xaction) {
        action = xaction;
        is_end = false;
        transform.SetParent(GM.action_queue.transform);
        // bg_img.sprite = FM.GetXActionCardBg(this);
        // work_img.sprite = action.actor.work_sprite;
        actor_word_text.color = FM.GetCampColor(action.actor.camp);
        actor_word_text.text = action.action_name;
        var word_len = action.action_name.Length;
        if (word_len == 1) actor_word_text.fontSize = 60;
        else if (word_len == 2) actor_word_text.fontSize = 54;
        else if (word_len >= 3 && word_len <= 4) actor_word_text.fontSize = 48;
        else actor_word_text.fontSize = 38;
    }
    public void End() {
        is_end = true;
        var target_pos = rect_trans.anchoredPosition;
        target_pos.x -= 500;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f);
        sequence.Append(rect_trans.DOAnchorPos(target_pos, 0.5f).SetEase(Ease.InOutSine));
        sequence.OnComplete(() => Destroy(gameObject));
    }
    public void MoveActionCard(Vector2 target_pos, float duration = 0.5f) {
        if (new Vector2(transform.position.x, transform.position.y) != target_pos) {
            if (moving_tween != null) {
                moving_tween.Kill();
            }
            moving_tween = rect_trans.DOAnchorPos(target_pos, duration).SetEase(Ease.InOutSine).OnComplete(() => moving_tween = null);
        }
    }
    public virtual void SetActing(int round_id, int act_id) { }
}
