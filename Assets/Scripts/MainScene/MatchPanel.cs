using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class MatchPanel : MonoBehaviour {
    public RectTransform match_bg0_trans;
    public RectTransform match_bg1_trans;
    public GameObject match_button_go;
    public GameObject match_cancel_button_go;
    public TMP_Text score_text;
    public TMP_Text match_time_text;
    public TMP_Text matching_tip_text;
    public TMP_Text user_text;
    public CanvasGroup canvas_group;

    private IEnumerator matching_coro;
    private Sequence matching_tip_enter_sequence;
    private Sequence match_bg0_sequence;
    private Sequence match_bg1_sequence;
    public void Start() {
        score_text.text = GrpcService.Ins.user_info.game_score.ToString();
        user_text.text = GrpcService.Ins.user_info.nick_name;
        GetUserInfo();
        BGEffect();
        StarBG.Ins.Play();
        BeforeMatchPanel();
        canvas_group.alpha = 0;
        DOTween.To(() => canvas_group.alpha,
                   (x) => canvas_group.alpha = x,
                   1, 2f);
    }
    public void BeforeMatchPanel() {
        match_time_text.text = "";
        matching_tip_text.text = "";
        match_button_go.GetComponent<Button>().interactable = true;
        match_button_go.SetActive(true);
        match_cancel_button_go.SetActive(false);
        TweenKill();
        if (matching_coro != null) {
            StopCoroutine(matching_coro);
            matching_coro = null;
        }
    }
    public void MatchingPanel() {
        match_cancel_button_go.GetComponent<Button>().interactable = true;
        match_button_go.SetActive(false);
        match_cancel_button_go.SetActive(true);
        TweenPlay();
        matching_tip_text.text = "- 匹 配 中 -";
        matching_coro = MatchTimeIEnumerator();
        StartCoroutine(matching_coro);
    }
    public void GetUserInfo() {
        void LoadUserInfo(string content) {
            var data = JsonUtility.FromJson<GrpcUserInfoContent>(content);
            score_text.text = data.rating;
            user_text.text = data.nick_name;
        }
        GrpcService.Ins.GetUserInfo(new GrpcService.GetUserInfoCallback(LoadUserInfo));
    }
    public void Match() {
        match_button_go.GetComponent<Button>().interactable = false;
        void LoadCallback(bool is_success, string content) {
            var match_content = JsonUtility.FromJson<GrpcMatchContent>(content);
            MatchCallback(match_content.match_result, match_content.player_name_list);
        }
        GrpcService.Ins.Match(new GrpcService.MatchCallback(LoadCallback));
    }
    public void MatchCancel() {
        match_cancel_button_go.GetComponent<Button>().interactable = false;
        GrpcService.Ins.CancelMatch();
    }
    public void MatchCallback(string match_result, List<string> player_name_list) {
        if (match_result == "SUCCESS") {
            Debug.Log("MatchCallback: SUCCESS");
            StartCoroutine(AsyncLoadcGameScene());
            LoadingPanel.Ins.Init(player_name_list);
        }
        else if (match_result == "START") {
            MatchingPanel();
        }
        else if (match_result == "CANCEL") {
            BeforeMatchPanel();
        }
        else if (match_result == "TIMEOUT") {
            BeforeMatchPanel();
        }
    }
    IEnumerator MatchTimeIEnumerator() {
        int matching_time = 0;
        while (matching_time <= 86400) {
            match_time_text.text = (matching_time / 60).ToString("D2") + ":" + (matching_time % 60).ToString("D2");
            yield return new WaitForSeconds(1f);
            matching_time++;
        }
    }
    IEnumerator AsyncLoadcGameScene() {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Buil Settings.
        matching_tip_text.text = "- 匹配成功 -";
        StarBG.Ins.Pause();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
    public void TweenPlay() {
        matching_tip_enter_sequence.Restart();
        match_bg0_sequence.Restart();
        match_bg1_sequence.Restart();
    }
    public void TweenKill() {
        matching_tip_enter_sequence.Rewind();
        match_bg0_sequence.Rewind();
        match_bg1_sequence.Rewind();
    }
    private void BGEffect() {
        // simulationSpeed

        // matching_tip_text's Tween
        matching_tip_enter_sequence = DOTween.Sequence();
        var tween100_0 = DOTween.To(() => matching_tip_text.fontSize,
                                    x => matching_tip_text.fontSize = x,
                                    36, 0.8f).SetEase(Ease.InQuad);
        var tween100_1 = DOTween.To(() => matching_tip_text.color,
                                    x => matching_tip_text.color = x,
                                    new Color(1, 1, 1, 1), 0.8f).SetEase(Ease.InQuad);
        matching_tip_enter_sequence.Append(tween100_0);
        matching_tip_enter_sequence.Join(tween100_1);
        matching_tip_enter_sequence.Pause();



        Image match_bg0_image = match_bg0_trans.GetComponent<Image>();
        Image match_bg1_image = match_bg1_trans.GetComponent<Image>();

        // match_bg0's Tween
        var tween0_0 = match_bg0_trans.DOLocalRotate(new Vector3(0, 0, -30), 0.4f, RotateMode.WorldAxisAdd)
                                      .SetEase(Ease.InQuad);
        var tween0_1 = DOTween.To(() => match_bg0_trans.localScale.x,
                                x => match_bg0_trans.localScale = new Vector3(x, x, x),
                                1.06f, 0.4f).SetEase(Ease.InQuad);
        var tween0_2 = match_bg0_image.DOColor(new Color(1, 1, 1, 0.8f), 0.4f)
                                      .SetEase(Ease.InQuad);

        var tween1 = match_bg0_trans.DOLocalRotate(new Vector3(0, 0, 420), 1.5f, RotateMode.WorldAxisAdd)
                                    .SetEase(Ease.InOutCubic);

        var tween2_0 = match_bg0_trans.DOLocalRotate(new Vector3(0, 0, -30), 0.4f, RotateMode.WorldAxisAdd)
                                      .SetEase(Ease.InQuad);
        var tween2_1 = DOTween.To(() => match_bg0_trans.localScale.x,
                                x => match_bg0_trans.localScale = new Vector3(x, x, x),
                                1f, 0.4f).SetEase(Ease.InQuad);
        var tween2_2 = match_bg0_image.DOColor(new Color(1, 1, 1, 0.4f), 0.4f)
                                      .SetEase(Ease.InQuad);

        match_bg0_sequence = DOTween.Sequence();
        match_bg0_sequence.AppendInterval(5);
        match_bg0_sequence.Append(tween0_0);
        match_bg0_sequence.Join(tween0_1);
        match_bg0_sequence.Join(tween0_2);
        match_bg0_sequence.Append(tween1);
        match_bg0_sequence.Append(tween2_0);
        match_bg0_sequence.Join(tween2_1);
        match_bg0_sequence.Join(tween2_2);
        match_bg0_sequence.AppendInterval(5);
        match_bg0_sequence.SetLoops(-1);
        match_bg0_sequence.Pause();

        // match_bg1's Tween
        var tween10 = match_bg1_trans.DOLocalRotate(new Vector3(0, 0, -360), 4.6f, RotateMode.WorldAxisAdd)
                                     .SetEase(Ease.InOutCubic);
        match_bg1_sequence = DOTween.Sequence();
        match_bg1_sequence.AppendInterval(10);
        match_bg1_sequence.Append(tween10);
        match_bg1_sequence.AppendInterval(10);
        match_bg1_sequence.SetLoops(-1);
        match_bg1_sequence.Pause();

    }
}