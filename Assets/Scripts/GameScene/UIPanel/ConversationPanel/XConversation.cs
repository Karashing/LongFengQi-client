using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class XConversation : BaseBehaviour {
    public RectTransform rect_trans { get { return GetComponent<RectTransform>(); } }
    public TMP_Text user_name_text;
    public ConversationData data;
    public bool is_right;
    public XEmotion emotion;

    public void Init(ConversationData xdata) {
        data = xdata;
        is_right = data.user_name == GrpcService.Ins.user_info.nick_name;
        emotion = FM.LoadXEmotion(data.emotion, is_right);
        emotion.transform.SetParent(transform);
        user_name_text.text = xdata.user_name;
    }

}