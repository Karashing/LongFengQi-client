using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class PropertyUIContent : BaseBehaviour {
    public RectTransform rect_trans;
    public TMP_Text detail_text;
    public void Init(XActor xactor) {
        if (xactor is XChess xchess) {
            var delta_attack = xchess.cur_attack - xchess.attack;
            var delta_speed = xchess.CalRealSpeedByBuffs() - xchess.speed;
            detail_text.text = $"等级: <b>{xchess.level}</b><size=5><br><br></size>" +
                               $"攻击力: <b>{xchess.attack} <color=#81daf0>{delta_attack.ToString("+#;-#;+0")}</color></b><size=5><br><br></size>" +
                               $"生命值: <b>{xchess.hp}</b> /<b>{xchess.max_hp}</b><size=5><br><br></size>" +
                               $"速度: <b>{xchess.speed} <color=#5cacee>{delta_speed.ToString("+#;-#;+0")}</color></b><br>";
        }
        else if (xactor is XGrid xgrid) {
            var delta_speed = xactor.CalRealSpeedByBuffs() - xactor.speed;
            detail_text.text = $"等级: <b>{xgrid.level}</b><size=5><br><br></size>" +
                               $"速度: <b>{xactor.speed} <color=#5cacee>{delta_speed.ToString("+#;-#;+0")}</color></b><br>";
        }
        else {
            var delta_speed = xactor.CalRealSpeedByBuffs() - xactor.speed;
            detail_text.text = $"速度: <b>{xactor.speed} <color=#5cacee>{delta_speed.ToString("+#;-#;+0")}</color></b><br>";
        }
        rect_trans.sizeDelta = new Vector2(rect_trans.sizeDelta.x, detail_text.preferredHeight);
        gameObject.SetActive(true);
    }
    public void End() {
        gameObject.SetActive(false);
    }
}