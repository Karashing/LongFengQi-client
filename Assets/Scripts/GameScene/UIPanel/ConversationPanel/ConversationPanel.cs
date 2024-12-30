using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class ConversationPanel : BaseBehaviour {
    public Sprite enter_sprite, back_sprite;
    public GameObject panel_go;
    public Image main_button_img;
    public RectTransform conv_panel_trans;
    public RectTransform conv_content_trans;
    public List<XConversation> conv_list;

    public GameObject emotion_panel_go;
    public RectTransform emo_content_trans;

    private void Awake() {
        NM.conversation.AddCallback(AddConversation);
    }
    private void Start() {
        for (int i = 0; i < conv_content_trans.childCount; ++i) {
            Destroy(conv_content_trans.GetChild(i).gameObject);
        }
        for (int i = 0; i < emo_content_trans.childCount; ++i) {
            Destroy(emo_content_trans.GetChild(i).gameObject);
        }
        emo_content_trans.sizeDelta = new Vector2((GameData.emotion_count + 1) / 2 * 120, emo_content_trans.sizeDelta.y);
        for (int i = 0; i < GameData.emotion_count; ++i) {
            var emotion_ui = FM.LoadEmotionUI(i);
            emotion_ui.transform.SetParent(emo_content_trans);
            emotion_ui.rect_trans.localScale = Vector3.one;
        }
        Refresh();
        emotion_panel_go.SetActive(false);
        panel_go.SetActive(false);
    }
    public void AddConversation(ConversationData data) {
        if (panel_go.activeSelf == false) {
            if (data.user_name != GrpcService.Ins.user_info.nick_name) {
                var xconv_ui = FM.LoadXConversation(data, false);
                xconv_ui.transform.SetParent(transform);
                xconv_ui.rect_trans.anchoredPosition = new Vector2(0, 100);
                Sequence sequence = DOTween.Sequence();
                sequence.Append(xconv_ui.rect_trans.DOAnchorPos(new Vector2(0, 200), 1f));
                sequence.Join(xconv_ui.user_name_text.DOColor(new Color(1, 1, 1, 0), 1f));
                sequence.Join(xconv_ui.emotion.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 1f));
                sequence.OnComplete(() => Destroy(xconv_ui.gameObject));
            }
        }
        var xconv = FM.LoadXConversation(data, data.user_name == GrpcService.Ins.user_info.nick_name);
        conv_list.Add(xconv);
        xconv.transform.SetParent(conv_content_trans);
        xconv.rect_trans.localScale = Vector3.one;
        Refresh();
        conv_content_trans.anchoredPosition = new Vector2(conv_content_trans.anchoredPosition.x, Mathf.Max(conv_content_trans.sizeDelta.y - conv_panel_trans.sizeDelta.y, 0f));
        Debug.Log(conv_content_trans.sizeDelta.y);
        Debug.Log(conv_panel_trans.sizeDelta.y);
        Debug.Log(conv_content_trans.sizeDelta.y - conv_panel_trans.sizeDelta.y);
    }
    public void Refresh() {
        conv_content_trans.sizeDelta = new Vector2(conv_content_trans.sizeDelta.x, 100f * conv_list.Count);
    }
    public void OnClickEmotionButton() {
        emotion_panel_go.SetActive(!emotion_panel_go.activeSelf);
    }
    public void OnClickMainButton() {
        panel_go.SetActive(!panel_go.activeSelf);
        if (panel_go.activeSelf) {
            main_button_img.sprite = back_sprite;
        }
        else {
            main_button_img.sprite = enter_sprite;
        }
    }
}