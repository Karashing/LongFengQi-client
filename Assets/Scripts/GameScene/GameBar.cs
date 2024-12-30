using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameBar : BaseBehaviour {
    public TMP_Text time_text;
    private void Awake() {
        EM.round_time_change.AddListener(TimeChange);
    }
    private void TimeChange() {
        time_text.text = GameInfo.cur_round_time.ToString();
        StartCoroutine(TimeTextWarn(GameInfo.cur_round_time));
    }
    IEnumerator TimeTextWarn(int time_limit) {

        if (time_limit <= 5) {
            time_text.color = new Color(255, 0, 0);
            if (time_limit > 0) {
                yield return new WaitForSeconds(0.5f);
                time_text.color = new Color(255, 255, 255);
            }
        }
        else {
            time_text.color = new Color(255, 255, 255);

        }
    }

}
