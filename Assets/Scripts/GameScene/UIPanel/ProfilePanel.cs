using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class ProfilePanel : BaseBehaviour {
    public TMP_Text title_text, profile_text;
    public RectTransform content_trans;
    public ActorUI actorUI;

    public TMP_Text info_text;

    private void Awake() {
        EM.act_time_change.AddListener(UpdateInfoPanel);
        EM.coin_change.AddListener(UpdateInfoPanel);
        EM.bean_change.AddListener(UpdateInfoPanel);
    }

    public void Init(XActor xactor) {
        for (int i = 0; i < content_trans.childCount; ++i) {
            Destroy(content_trans.GetChild(i).gameObject);
        }
        actorUI = FM.LoadActorUI(xactor);
        actorUI.transform.SetParent(content_trans);
        actorUI.rect_trans.localScale = Vector3.one;
        actorUI.button.onClick.AddListener(() => GM.camera_moudle.FocusOnPosition(xactor.transform.position));
        if (xactor is XChess xchess) {
            title_text.text = $"<b>{xchess.rarity}★<br>Lv.{xchess.perform_level}</b>";
            var delta_attack = xchess.cur_attack - xchess.attack;
            var delta_speed = xchess.CalRealSpeedByBuffs() - xchess.speed;
            profile_text.text = $"攻击力: <b>{xchess.attack} <color=#81daf0>{delta_attack.ToString("+#;-#;+0")}</color></b><br>" +
                                $"生命值: <b>{xchess.hp}</b> /<b>{xchess.max_hp}</b><br>" +
                                $"速度: <b>{xchess.speed} <color=#5cacee>{delta_speed.ToString("+#;-#;+0")}</color></b><br>";
        }
        else if (xactor is XGrid xgrid) {
            title_text.text = $"<b>「{xgrid.word}」<br>Lv.{xgrid.perform_level}</b>";
            var delta_speed = xactor.CalRealSpeedByBuffs() - xactor.speed;
            profile_text.text = $"速度: <b>{xactor.speed} <color=#5cacee>{delta_speed.ToString("+#;-#;+0")}</color></b><br>";
        }
        else {
            title_text.text = $"<b>「{xactor.word}」<br>Lv.{xactor.perform_level}</b>";
            var delta_speed = xactor.CalRealSpeedByBuffs() - xactor.speed;
            profile_text.text = $"速度: <b>{xactor.speed} <color=#5cacee>{delta_speed.ToString("+#;-#;+0")}</color></b><br>";
        }
    }
    public void UpdateInfoPanel() {
        info_text.text = $"总行动值: {GameInfo.cur_act_time}<br>"
                       + $"当前回合: {GameInfo.cur_round}<br>"
                       + $"金币: {GameInfo.coin}<br>"
                       + $"能量: {GameInfo.bean}/{GameInfo.max_bean}<br>";
    }
}