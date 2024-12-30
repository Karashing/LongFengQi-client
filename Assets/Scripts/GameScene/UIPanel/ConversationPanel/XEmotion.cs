using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class XEmotion : BaseBehaviour {
    public RectTransform rect_trans;
    public Animator anim;
    public void Init(bool is_right) {
        if (is_right) {
            rect_trans.pivot = new Vector2(1, 0);
            rect_trans.anchorMax = new Vector2(1, 0);
            rect_trans.anchorMin = new Vector2(1, 0);
        }
        else {
            rect_trans.pivot = new Vector2(0, 0);
            rect_trans.anchorMax = new Vector2(0, 0);
            rect_trans.anchorMin = new Vector2(0, 0);
        }

    }

}