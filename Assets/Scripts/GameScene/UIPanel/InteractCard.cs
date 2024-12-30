using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Runtime.Serialization;
using UnityEngine.EventSystems;

public class InteractCard : BaseBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public RectTransform rect_trans;
    public Button button;
    public TMP_Text card_name_text, card_describe_text;
    public Image bg_img, symbol_img;
    public RectTransform describe_content_rect_trans;
    public bool enable;
    public XSkill skill;

    // InteractCard
    public void Init(XSkill xskill) {
        skill = xskill;
        card_name_text.text = xskill.name();
        card_describe_text.text = xskill.role();
        if (describe_content_rect_trans != null)
            describe_content_rect_trans.sizeDelta = new Vector2(describe_content_rect_trans.sizeDelta.x, card_describe_text.preferredHeight);
        symbol_img.sprite = xskill.symbol_sprite;

        skill.refresh_event.AddListener(OnRefresh);
        enable = xskill.IsEnable();
        button.interactable = enable;
    }
    public void BeSelected() {
        if (!enable) return;
        if (GM.interact_queue.selected_card == this) return;
        if (GM.interact_queue.selected_card != null)
            GM.interact_queue.selected_card.CancelSelected();
        GM.interact_queue.selected_card = this;
        if (skill.interact_mode == 0) {
            rect_trans.anchoredPosition = new Vector2(rect_trans.anchoredPosition.x, 20);
        }
        rect_trans.localScale = new Vector3(1.1f, 1.1f);
        Debug.Log("[" + card_name_text.text + "]");
        skill.BeSelect();
    }
    public void CancelSelected() {
        if (skill.interact_mode == 0) {
            rect_trans.anchoredPosition = new Vector2(rect_trans.anchoredPosition.x, 0);
        }
        rect_trans.localScale = new Vector3(1f, 1f);
        skill.CancelSelect();
    }
    public void OnConfirm() {
        if (!enable) return;
        if (GM.interact_queue.selected_card != this)
            BeSelected();
        skill.ConfirmSkill();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (GM.interact_queue.selected_card != this && enable) {
            rect_trans.anchoredPosition = new Vector2(rect_trans.anchoredPosition.x, rect_trans.anchoredPosition.y + 10);
            rect_trans.localScale = new Vector3(1.05f, 1.05f);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (GM.interact_queue.selected_card != this && enable) {
            rect_trans.anchoredPosition = new Vector2(rect_trans.anchoredPosition.x, rect_trans.anchoredPosition.y - 10);
            rect_trans.localScale = new Vector3(1f, 1f);
        }
    }
    private void OnRefresh() {
        Init(skill);
    }
}


