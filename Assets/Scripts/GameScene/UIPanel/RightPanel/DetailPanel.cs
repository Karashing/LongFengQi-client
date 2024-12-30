using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using Unity.VisualScripting;
using System.IO;

// TODO
public class DetailPanel : BaseBehaviour {
    public RectTransform rect_trans;
    public GameObject enter_button_go;
    private bool is_showing = false;
    private Tween switch_tween = null;

    public ScrollRect actor_uis_rect;
    public ButtonGroup actors_camp_button_group;
    public RectTransform self_actors_content_trans, neutral_actors_content_trans, enemy_actors_content_trans;

    public RectTransform actor_content_trans;
    public TMP_Text title_text;
    public ScrollRect detail_rect;
    public ButtonGroup actor_module_button_group;
    public PropertyUIContent property_content;
    public BuffUIContent buff_content;
    public SkillUIContent skill_content;
    public XActor cur_show_actor;

    public void Init() {
        if (cur_show_actor == null) {
            var actor_uis = OnSelectCamp(XCamp.SELF);
            if (actor_uis.Count > 0) OnSelectActor(actor_uis[0].actor);
        }

        if (is_showing == false) {
            if (switch_tween != null) {
                switch_tween.Kill();
                switch_tween = null;
            }
            switch_tween = rect_trans.DOAnchorPosX(0f, 0.5f).OnComplete(() => {
                is_showing = true;
            });
            enter_button_go.SetActive(false);
        }
    }
    public void OnSelectSelfCamp() { OnSelectCamp(XCamp.SELF); }
    public void OnSelectNeutralCamp() { OnSelectCamp(XCamp.NEUTRAL); }
    public void OnSelectEnemyCamp() { OnSelectCamp(XCamp.ENEMY); }
    public List<ActorUI> OnSelectCamp(XCamp xcamp) {
        self_actors_content_trans.gameObject.SetActive(false);
        neutral_actors_content_trans.gameObject.SetActive(false);
        enemy_actors_content_trans.gameObject.SetActive(false);

        var button_id = 0;
        RectTransform content_trans = null;
        if (xcamp == XCamp.SELF) {
            button_id = 0;
            content_trans = self_actors_content_trans;
        }
        else if (xcamp == XCamp.NEUTRAL) {
            button_id = 1;
            content_trans = neutral_actors_content_trans;
        }
        else {
            button_id = 2;
            content_trans = enemy_actors_content_trans;
        }
        actors_camp_button_group.OnSelect(button_id);
        for (int i = 0; i < content_trans.childCount; ++i) {
            Destroy(content_trans.GetChild(i).gameObject);
        }
        var actor_uis = new List<ActorUI>();
        foreach (var xactor in GameInfo.actor_dict.Values) {
            if (xactor.camp == xcamp && (xactor.in_action || (xactor.camp == XCamp.SELF && xactor is XChess))) {
                var actor_ui = FM.LoadActorUI(xactor);
                actor_ui.button.onClick.AddListener(() => OnSelectActor(xactor));
                actor_ui.multi_font_size = 0.6f;
                actor_uis.Add(actor_ui);
            }
        }
        actor_uis.Sort((x, y) => x.CompareTo(y));
        foreach (var xactor in actor_uis) {
            xactor.transform.SetParent(content_trans);
            xactor.rect_trans.localScale = Vector3.one;
        }
        content_trans.sizeDelta = new Vector2(65 * actor_uis.Count + 15, content_trans.sizeDelta.y);
        content_trans.gameObject.SetActive(true);
        actor_uis_rect.content = content_trans;
        return actor_uis;
    }
    public void OnSelectActor(XActor xactor) {
        cur_show_actor = xactor;
        for (int i = 0; i < actor_content_trans.childCount; ++i) {
            Destroy(actor_content_trans.GetChild(i).gameObject);
        }
        var actor_ui = FM.LoadActorUI(xactor);
        actor_ui.transform.SetParent(actor_content_trans);
        actor_ui.rect_trans.localScale = Vector3.one;
        if (xactor is XChess xchess) {
            title_text.text = $"<b>{xchess.rarity}</b>★<br>Lv.{xchess.perform_level}";
        }
        else if (xactor is XGrid xgrid) {
            title_text.text = $"<b>「{xgrid.word}」<br>Lv.{xgrid.perform_level}</b>";
        }
        else {
            title_text.text = $"<b>「{xactor.word}」<br>Lv.{xactor.perform_level}</b>";
        }
        OnSelectActorModule(0);
    }
    public void OnSelectActorModule(int module_id) { // [0,2]
        actor_module_button_group.OnSelect(module_id);
        property_content.End();
        buff_content.End();
        skill_content.End();
        if (module_id == 0) {
            property_content.Init(cur_show_actor);
            detail_rect.content = property_content.rect_trans;
        }
        else if (module_id == 1) {
            buff_content.Init(cur_show_actor);
            detail_rect.content = buff_content.rect_trans;
        }
        else {
            skill_content.Init(cur_show_actor);
            detail_rect.content = skill_content.rect_trans;
        }

    }
    public void Back() {
        if (is_showing) {
            if (switch_tween != null) {
                switch_tween.Kill();
                switch_tween = null;
            }
            switch_tween = rect_trans.DOAnchorPosX(500f, 0.5f).OnComplete(() => {
                is_showing = false;
                enter_button_go.SetActive(true);
            });
        }
    }
}