using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using TMPro;


public class TipTextBG : BaseBehaviour {
    public TMP_Text text;
    private void Awake() {
        EM.actors_change.AddListener(UpdateText);
        EM.exp_change.AddListener(UpdateText);
        EM.round_change.AddListener(UpdateText);
    }
    private void UpdateText() {
        text.text = $"{GameInfo.self_in_board_countable_chess_num}/{GameInfo.can_inboard_chess_num}";
    }
}