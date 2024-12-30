using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using ToolI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ShopChessTab : BaseBehaviour {
    public RectTransform rect_trans;
    public Sprite buy_sprite, done_sprite;
    public Image bg_img, chess_bg_img, chess_work_img;
    public Button buy_button;
    public Image buy_button_img;
    public TMP_Text chess_word_text;
    public TMP_Text atk_text, hp_text;
    public TMP_Text label_text;
    public TMP_Text detail_word_text, detail_text;
    public GameObject default_panel, detail_panel;
    public RectTransform detail_content_rect_trans;
    public TMP_Text coin_text;
    public XChess chess;
    public void Init(XChess xchess) {
        chess = xchess;
        bg_img.sprite = FM.GetShopTabBGSprite(chess.rarity);
        chess_bg_img.sprite = chess.chess_bg_spr.sprite;
        chess_work_img.sprite = chess.work_sprite;
        chess_word_text.text = chess.word;
        detail_word_text.text = chess.word;

        buy_button.interactable = true;
        buy_button_img.sprite = buy_sprite;

        atk_text.text = chess.attack.ToString();
        hp_text.text = chess.max_hp.ToString();
        label_text.text = chess.career;
        coin_text.text = "x" + chess.rarity.ToString();

        detail_text.text = $"<b>【{chess.word}】 {chess.rarity}星<br>" +
                           $" Lv.{chess.perform_level}</b><br>" +
                           $"攻击力：{chess.attack}<br>" +
                           $"生命值：{chess.max_hp}<br>" +
                           $"<br>";

        foreach (var skill in chess.skills) {
            detail_text.text += $"【{skill.name()}】<br>" +
                                $"{skill.role()}<br>" +
                                $"<br>";
        }
        detail_content_rect_trans.sizeDelta = new Vector2(detail_content_rect_trans.sizeDelta.x, detail_text.preferredHeight);
        default_panel.SetActive(true);
        detail_panel.SetActive(false);
    }
    public void SwitchPanel() {
        if (default_panel.activeSelf) {
            default_panel.SetActive(false);
            detail_panel.SetActive(true);
        }
        else {
            default_panel.SetActive(true);
            detail_panel.SetActive(false);
        }
    }
    public void BuyChess() {
        if (CanBuyChess()) {
            buy_button.interactable = false;
            buy_button_img.sprite = done_sprite;
            coin_text.text = "";

            NM.build_actor.Send(new ActorData(new ChessData(
                serverId: Generate.GenerateId(),
                chessType: chess.type,
                chessLevel: 0,
                xcamp: XCamp.SELF,
                chessPositionType: ChessPositionType.random_prepare_grid
            )));
        }
    }
    public bool CanBuyChess() {
        if (GameInfo.coin < chess.rarity) return false;
        var chess_list = GameInfo.GetChesss(chess.type, XCamp.SELF);
        if (chess_list.Count > 0) {
            foreach (var xchess in chess_list) {
                if (xchess.level < xchess.max_level) return true;
            }
        }
        if (GameInfo.self_prepare_chess_num >= GameInfo.self_prepare_grid_num) return false;
        return true;
    }
}
