using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ToolI;
using TMPro;

public class ActionCard : XActionCard {
    public TMP_Text mil_text, action_id_text;
    public GameObject mil_go;

    public override void Init(XAction xaction) {
        base.Init(xaction);
        rect_trans.anchoredPosition = new Vector2(100, -1120);
    }

    public override void SetActing(int round_id, int act_id) {
        bool is_acting = round_id == 0 && act_id == 0;
        mil_go.SetActive(!is_acting);
        if (!is_acting) {
            rect_trans.localScale = new(0.9f, 0.9f, 1f);
        }
        else {
            rect_trans.localScale = new(1f, 1f, 1f);
        }
        action_id_text.text = MathI.ToRotmanNumbers(round_id);
    }
    public void SetMilText(float delta_mil) {
        mil_text.text = Mathf.RoundToInt(delta_mil).ToString();
    }
}
