using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ToolI;
using LitJson;
using System.Linq;
using TMPro;

class test_tmp : MonoBehaviour {
    public TMP_Text text;
    public RectTransform rectTransform;

    void Start() {
        int x = 123, y = -1, z = 0;
        float xf = 1.87f, yf = -0.977f, zf = 0;
        text.text = $"{x.ToString("+#;-#;0")}\n{y.ToString("+#;-#;0")}\n{z.ToString("+#;-#;+0")}\n{xf.ToString("+#;-#;0")}\n{yf.ToString("+#;-#;0")}\n{zf.ToString("+#;-#;+0")}\n";
    }

    // void Start() {
    //     //代码赋值文本
    //     text.text = "好!<br>很好!<br>非常好!非常好!非常好!非常好!123123<br><br>";

    //     //获取文本的高度
    //     float preferredHeight = text.preferredHeight;
    //     Debug.Log(preferredHeight);


    //     //组件高度根据实际文本高度自适应
    //     rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, preferredHeight);
    // }

    // void Start() {
    //     //代码赋值文本
    //     text.text = "好!\n很好!\n非常好!";

    //     //获取文本的高度
    //     float preferredHeight = text.preferredHeight;

    //     //获取文本的RectTransform
    //     RectTransform rectTransform = text.GetComponent<RectTransform>();

    //     //组件高度根据实际文本高度自适应
    //     rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, preferredHeight);
    // }

}