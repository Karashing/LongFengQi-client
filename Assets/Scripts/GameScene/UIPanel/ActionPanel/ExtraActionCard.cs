using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ToolI;
using TMPro;

public class ExtraActionCard : XActionCard {
    public GameObject action_id_go;
    public TMP_Text action_id_text;
    public RectTransform work_rect_trans;


    public override void Init(XAction xaction) {
        base.Init(xaction);
        rect_trans.anchoredPosition = new Vector2(-1000, 0);
    }
    public override void SetActing(int round_id, int act_id) {
        bool is_acting = act_id == 0;
        var word_len = action.action_name.Length;
        if (word_len == 1) actor_word_text.fontSize = 60;
        else if (word_len == 2) actor_word_text.fontSize = 54;
        else if (word_len >= 3 && word_len <= 4) actor_word_text.fontSize = 48;
        else actor_word_text.fontSize = 38;
        if (!is_acting) {
            rect_trans.sizeDelta = new Vector2(72f, 72f);
            rect_trans.localScale = new(0.9f, 0.9f, 1f);
            work_rect_trans.sizeDelta = new Vector2(72f, 72f);
            bg_img.sprite = FM.GetExtraActionCardSmallBg(action.actor);
            actor_word_text.fontSize -= 8;
            action_id_go.SetActive(false);
        }
        else {
            rect_trans.sizeDelta = new Vector2(180f, 72f);
            rect_trans.localScale = new(1f, 1f, 1f);
            work_rect_trans.sizeDelta = new Vector2(108f, 108f);
            bg_img.sprite = FM.GetExtraActionCardBg(action.actor);
            action_id_go.SetActive(true);
            action_id_text.text = MathI.ToRotmanNumbers(0);
        }
    }
}
