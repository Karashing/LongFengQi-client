using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class GameoverPanel : BaseBehaviour {
    public TMP_Text game_result_text;
    public TMP_Text game_info_text;

    public void Init(GameRatingData rating_data) {
        if (GameInfo.game_result == GameResult.Win) {
            game_result_text.text = "WIN";
            game_result_text.color = new Color(1, 0.9375f, 0.75f);
        }
        else if (GameInfo.game_result == GameResult.Lose) {
            game_result_text.text = "LOSE";
            game_result_text.color = new Color(1, 0.25f, 0.25f);
        }
        else if (GameInfo.game_result == GameResult.Draw) {
            game_result_text.text = "DRAW";
            game_result_text.color = new Color(1, 0.65f, 0.3f);
        }
        string sign_text;
        if (rating_data.delta_rating >= 0) sign_text = "+";
        else sign_text = "-";
        game_info_text.text = $"<b>Rating: {rating_data.rating}（{sign_text}{Mathf.Abs(rating_data.delta_rating)}）</b><br><br>"
                            + $"总行动值：{(int)GameInfo.cur_act_time}<br>"
                            + $"总回合数：{GameInfo.cur_round}";
        gameObject.SetActive(true);
    }
    public void Back() {
        GM.CloseGame();
    }
}