using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Runtime.Serialization;
using UnityEngine.EventSystems;
using ToolI;

public class BigSkillCard : BaseBehaviour {
    public RectTransform rect_trans;
    public Image fill_img, symbol_img;
    public ActorNormalUI actor_ui;
    public TMP_Text level_text, tip_text;
    public Button button;

    public Sprite fill_sprite, underfill_sprite;
    public XActor actor;
    public int interact_id;
    private Tween moving_tween;
    private bool is_round_switching;
    private bool prepare_to_release;

    private void Awake() {
        EM.round_switching.AddListener(RoundSwitching);
    }


    public void Init(XActor xactor) {
        actor = xactor;
        actor_ui.Init(actor, true);
        symbol_img.sprite = FM.GetSymbolSprite(actor.symbol_type);
        Debug.Log(symbol_img.sprite.name);
        actor.level_change.AddListener(OnLevelChange);
        actor.energy_change.AddListener(OnEnergyChange);
        OnLevelChange();
        OnEnergyChange();
        rect_trans.anchoredPosition = new Vector2(-1000f, 0);
        prepare_to_release = false;
    }
    public void MoveCard(Vector2 target_pos, float duration = 0.5f) {
        if (rect_trans.anchoredPosition != target_pos) {
            if (moving_tween != null) {
                moving_tween.Kill();
            }
            moving_tween = rect_trans.DOAnchorPos(target_pos, duration).SetEase(Ease.InOutSine).OnComplete(() => moving_tween = null);
        }
    }
    public void OnLevelChange() {
        level_text.text = MathI.ToRotmanNumbers(actor.perform_level);
    }
    public void OnEnergyChange() {
        fill_img.fillAmount = actor.energy / actor.max_energy;
        if (actor.energy >= actor.max_energy) {
            fill_img.sprite = fill_sprite;
        }
        else {
            prepare_to_release = false;
            fill_img.sprite = underfill_sprite;
        }
        InteractableButton();
    }
    public void SetInteractId(int id) {
        interact_id = id;
        tip_text.text = id.ToString();
    }
    public void OnConfirm() {
        // TODO
        prepare_to_release = true;
        InteractableButton();
        NM.add_extra_actor_action.Send(new(new(actor, actor.big_skill, GameInfo.cur_round, 0)));
    }
    public void RoundSwitching(bool doing) {
        is_round_switching = doing;
        InteractableButton();
    }
    public void Kill() {
        actor.level_change.RemoveListener(OnLevelChange);
        actor.energy_change.RemoveListener(OnEnergyChange);
        Destroy(gameObject);
    }

    void InteractableButton() {
        if (!is_round_switching && !prepare_to_release && actor.energy >= actor.max_energy) {
            button.interactable = true;
        }
        else {
            button.interactable = false;
        }
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha0 + interact_id) && button.interactable) {
            OnConfirm();
        }
    }


    public void OnPointerEnter(PointerEventData eventData) {
        // if (GM.interact_queue.selected_card != this && enable) {
        //     rect_trans.anchoredPosition = new Vector2(rect_trans.anchoredPosition.x, rect_trans.anchoredPosition.y + 10);
        //     rect_trans.localScale = new Vector3(1.05f, 1.05f);
        // }
    }

    public void OnPointerExit(PointerEventData eventData) {
        // if (GM.interact_queue.selected_card != this && enable) {
        //     rect_trans.anchoredPosition = new Vector2(rect_trans.anchoredPosition.x, rect_trans.anchoredPosition.y - 10);
        //     rect_trans.localScale = new Vector3(1f, 1f);
        // }
    }
}


