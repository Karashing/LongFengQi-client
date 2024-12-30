using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class EmotionUI : BaseBehaviour {
    public RectTransform rect_trans { get { return GetComponent<RectTransform>(); } }
    public Image img;
    public int emotion_id;
    public void Init(int xemotion_id) {
        emotion_id = xemotion_id;
        img.sprite = FM.GetEmotionSprite(emotion_id);
    }
    public void OnSelect() {
        new ConversationData(emotion_id);
        NM.conversation.Send(new(emotion_id));
    }

}