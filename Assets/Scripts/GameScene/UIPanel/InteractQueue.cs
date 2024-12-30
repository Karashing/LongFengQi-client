using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class InteractQueue : BaseBehaviour {
    public RectTransform content_trans;
    public RectTransform functional_content_trans;
    private List<InteractCard> cards = new List<InteractCard>();
    private List<InteractCard> card1s = new List<InteractCard>();
    public XActor actor;
    public InteractCard selected_card;
    public XSkill selected_skill {
        get {
            if (selected_card == null) return null;
            return selected_card.skill;
        }
    }
    // InteractQueue
    public void Init(XActor xactor, List<XSkill> xskills) {
        actor = xactor;
        selected_card = null;
        for (int i = 0; i < content_trans.childCount; ++i) {
            Destroy(content_trans.GetChild(i).gameObject);
        }
        for (int i = 0; i < functional_content_trans.childCount; ++i) {
            Destroy(functional_content_trans.GetChild(i).gameObject);
        }

        cards = new List<InteractCard>();
        card1s = new List<InteractCard>();
        for (int i = 0; i < xskills.Count; ++i) {
            var skill = xskills[i];
            if (skill.interact_mode == 0) {
                var card = FM.LoadInteractCard(skill, skill.interact_mode);
                card.transform.SetParent(content_trans);
                card.rect_trans.anchoredPosition = new Vector2(190.5f + 341f * i, 0);
                card.rect_trans.localScale = new Vector3(1f, 1f);
                if (selected_card == null && card.enable) {
                    Debug.Log("{" + skill.name() + "}");
                    card.BeSelected();
                }
                cards.Add(card);
            }
            else {
                skill.Init();
                var card = FM.LoadInteractCard(skill, skill.interact_mode);
                card.transform.SetParent(functional_content_trans);
                card.rect_trans.localScale = new Vector3(1f, 1f);
                card1s.Add(card);
            }
        }
        content_trans.sizeDelta = new Vector2(341 * cards.Count + 60, content_trans.sizeDelta.y);
        content_trans.gameObject.SetActive(true);
        functional_content_trans.gameObject.SetActive(true);
        if (selected_card == null) {
            XSkill.ExecuteNoneSkill(actor.server_id, actor.act_round);
        }
    }
    public void EndInteract() {
        if (selected_card)
            selected_card.CancelSelected();
        actor = null;
        selected_card = null;
        content_trans.gameObject.SetActive(false);
        functional_content_trans.gameObject.SetActive(false);
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            GetLeftEnableCard().BeSelected();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            GetRightEnableCard().BeSelected();
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
            if (selected_card)
                selected_skill.ConfirmSkill();
        }
    }
    InteractCard GetLeftEnableCard() {
        InteractCard last_card = null;
        for (int i = 0; i < cards.Count; ++i) {
            if (cards[i].enable) {
                if (selected_card == cards[i]) {
                    if (last_card != null) return last_card;
                }
                last_card = cards[i];
            }
        }
        return last_card;
    }
    InteractCard GetRightEnableCard() {
        InteractCard last_card = null;
        for (int i = cards.Count - 1; i >= 0; --i) {
            if (cards[i].enable) {
                if (selected_card == cards[i]) {
                    if (last_card != null) return last_card;
                }
                last_card = cards[i];
            }
        }
        return last_card;
    }
}