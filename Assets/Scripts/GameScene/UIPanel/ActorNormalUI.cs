using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ActorNormalUI : BaseBehaviour {
    public RectTransform rect_trans { get { return GetComponent<RectTransform>(); } }
    public Image actor_bg_img, actor_work_img;
    public TMP_Text actor_word_text;

    public virtual void Init(XActor xactor, bool need_color = false) {
        if (actor_bg_img != null)
            actor_bg_img.gameObject.SetActive(true);
        if (xactor is XChess chess) {
            if (actor_bg_img != null)
                actor_bg_img.sprite = chess.chess_bg_spr.sprite;
        }
        else if (xactor is XGrid grid) {
            if (actor_bg_img != null)
                actor_bg_img.sprite = grid.sprite_renderer.sprite;
        }
        else {
            if (actor_bg_img != null)
                actor_bg_img.gameObject.SetActive(false);
        }
        actor_work_img.sprite = xactor.work_sprite;
        actor_word_text.text = xactor.word;
        if (need_color) {
            actor_word_text.color = FM.GetCampColor(xactor.camp);
        }
    }
    public void SetPosition(Vector3 xpos) {
        transform.position = xpos;
    }
    public void Kill() {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}