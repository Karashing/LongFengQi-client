using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class BuffUIContent : BaseBehaviour {
    public RectTransform rect_trans;
    public TMP_Text detail_text;
    public void Init(XActor xactor) {
        detail_text.text = "";
        foreach (XBuff xbuff in xactor.buffs) {
            detail_text.text += $"「<b><color=#{FM.GetCampColorHex(xbuff.owner_actor.camp)}>{xbuff.owner_actor.word}</color></b>」 <size=18>{xbuff.describe}</size><size=7><br><br></size>";
        }
        rect_trans.sizeDelta = new Vector2(rect_trans.sizeDelta.x, detail_text.preferredHeight);
        gameObject.SetActive(true);
    }
    public void End() {
        gameObject.SetActive(false);
    }
}