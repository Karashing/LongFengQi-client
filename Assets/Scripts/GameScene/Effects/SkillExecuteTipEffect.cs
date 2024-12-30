using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SkillExecuteTipEffect : BaseBehaviour {
    public RectTransform rect_trans;
    public ActorSimpleUI actor_ui;
    public TMP_Text skill_name_text;
    public Vector2 target_pos;
    private Sequence tween;
    public void Init(XActor xactor, XSkill xskill) {
        actor_ui.Init(xactor);
        skill_name_text.text = xskill.name();
        skill_name_text.color = new(1f, 1f, 1f, 0f);
        tween = null;
    }
    public void Play() {
        Sequence seq = DOTween.Sequence();
        rect_trans.anchoredPosition = new(target_pos.x, target_pos.y - 500);
        rect_trans.localScale = Vector3.one;
        seq.Append(rect_trans.DOAnchorPos(target_pos, 0.4f));
        seq.Join(actor_ui.GetEnterTween());
        seq.Join(skill_name_text.DOColor(Color.white, 0.4f));
        seq.Append(rect_trans.DOAnchorPos(target_pos + new Vector2(0, 50f), 1.2f));
        seq.Insert(1.4f, actor_ui.GetExitTween());
        seq.Insert(1.4f, skill_name_text.DOColor(new(1f, 1f, 1f, 0f), 0.2f));
        seq.AppendCallback(End);
        tween = seq;
        gameObject.SetActive(true);
    }
    public void End() {
        if (tween.IsActive())
            tween.Kill();
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
