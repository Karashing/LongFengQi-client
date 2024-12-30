using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CloneChess : BaseBehaviour {
    public SpriteRenderer chess_bg_spr, chess_work_spr;
    public TMP_Text chess_word_text;

    public virtual void Init(XChess xchess) {
        chess_bg_spr.sprite = xchess.chess_bg_spr.sprite;
        chess_work_spr.sprite = xchess.work_sprite;
        chess_word_text.text = xchess.word;
        chess_word_text.color = FM.GetCampColor(xchess.camp);
    }
    public void SetPosition(Vector3 xpos) {
        transform.position = xpos;
    }
    public void Kill() {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}