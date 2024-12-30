using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

// TODO
public class SkillUIContent : BaseBehaviour {
    public RectTransform rect_trans;
    public TMP_Text detail_text;
    public void Init(XActor xactor) {
        detail_text.text = "";
        foreach (XSkill xskill in xactor.skills) {
            detail_text.text += $"「<b>{xskill.name()}</b>」 <size=18>{xskill.role()}</size><size=7><br><br></size>";
        }
        rect_trans.sizeDelta = new Vector2(rect_trans.sizeDelta.x, detail_text.preferredHeight);
        gameObject.SetActive(true);
    }
    public void End() {
        gameObject.SetActive(false);
    }
}