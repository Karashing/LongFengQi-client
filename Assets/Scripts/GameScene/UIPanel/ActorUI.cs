using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ActorUI : BaseBehaviour {
    public RectTransform rect_trans { get { return GetComponent<RectTransform>(); } }
    public Image chess_bg_img, chess_work_img;
    public TMP_Text chess_word_text;
    public Button button;
    public XActor actor;
    public GameObject extra_go;
    public Image extra_img;
    public TMP_Text extra_script_text;
    private float _multi_font_size = 1f;
    public float multi_font_size {
        get { return _multi_font_size; }
        set {
            chess_word_text.fontSize *= value / _multi_font_size;
            extra_script_text.fontSize *= value / _multi_font_size;
            _multi_font_size = value;
        }
    }

    public virtual void Init(XActor xactor) {
        actor = xactor;
        _multi_font_size = 1f;
        extra_go.SetActive(false);
        if (xactor is XChess chess) {
            if (chess_bg_img != null)
                chess_bg_img.sprite = chess.chess_bg_spr.sprite;
            chess_work_img.sprite = chess.work_sprite;
            chess_word_text.text = chess.word;
            chess_word_text.color = FM.GetCampColor(chess.camp);
            if (!chess.in_action) {
                extra_script_text.text = "准备中";
                extra_img.color = new Color(0.2f, 0.2f, 0.2f, 0.6f);
                extra_go.SetActive(true);
            }
        }
        else if (xactor is XGrid grid) {
            if (chess_bg_img != null)
                chess_bg_img.sprite = grid.sprite_renderer.sprite;
            chess_work_img.sprite = grid.work_sprite;
            chess_word_text.text = grid.word;
            chess_word_text.color = FM.GetCampColor(grid.camp);
        }
        else {
            chess_work_img.sprite = xactor.work_sprite;
            chess_word_text.text = xactor.word;
            chess_word_text.color = FM.GetCampColor(xactor.camp);
        }
        if (GameInfo.cur_act_actor == xactor) {
            extra_script_text.text = "行动中";
            extra_img.color = new Color(0f, 1f, 0.375f, 0.6f);
            extra_go.SetActive(true);
        }
    }
    public int CompareTo(ActorUI xactorUI) {
        var xactor = xactorUI.actor;

        int _type, x_type;
        if (actor is XChess) _type = 0;
        else if (actor is XGrid) _type = 1;
        else _type = 2;
        if (xactor is XChess) x_type = 0;
        else if (xactor is XGrid) x_type = 1;
        else x_type = 2;

        if (_type != x_type) return _type.CompareTo(x_type);
        if (actor is XChess) {
            _type = actor.in_action ? 0 : 1;
            x_type = xactor.in_action ? 0 : 1;
            if (_type != x_type) return _type.CompareTo(x_type);
        }

        return long.Parse(actor.server_id).CompareTo(long.Parse(xactor.server_id));
    }
}