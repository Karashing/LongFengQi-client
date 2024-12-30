using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ToolI;
using System.Linq;

public class ShopModule : BaseBehaviour {
    public GameObject shop_panel;
    public Button shop_button;
    public Button exp_button;
    public Button refresh_button;
    public Image shop_button_image;
    public TMP_Text shop_button_coin_text;
    public TMP_Text exp_button_coin_text;
    public TMP_Text exp_button_level_text;
    public TMP_Text refresh_button_coin_text;
    public Transform tabs_trans;
    public ShopIncomePanel shop_income_panel;

    private bool is_activate = false;
    public ChessShop shop;
    public Dictionary<int, List<XChess>> chess_rarity_dict = new Dictionary<int, List<XChess>>();

    public void Awake() {
        // EM.coin_change.AddListener(UpdateInfo);
        // EM.exp_change.AddListener(UpdateInfo);
    }
    public void Init(ChessShop xshop) {
        shop_income_panel.Init();

        foreach (var xchess in GameData.chess_dict.Values) {
            if (!chess_rarity_dict.ContainsKey(xchess.rarity)) {
                chess_rarity_dict.Add(xchess.rarity, new List<XChess>());
            }
            if (xchess.can_buy)
                chess_rarity_dict[xchess.rarity].Add(xchess);
        }
        shop = xshop;
        GameInfo.shop_refresh_times = 0;
        refresh_button_coin_text.text = GameInfo.shop_refresh_coin.ToString();
        shop_button.interactable = true;
        Refresh(true);
        Activate(1);
    }
    public void End() {
        Activate(-1);
        shop_button.interactable = false;
    }
    public void Refresh(bool is_free) {
        if (GameInfo.coin >= GameInfo.shop_refresh_coin) {
            if (!is_free) {
                GameInfo.coin -= GameInfo.shop_refresh_coin;
                GameInfo.shop_refresh_times += 1;
                refresh_button_coin_text.text = GameInfo.shop_refresh_coin.ToString();
            }
            int[] chess_ids = GameInfo.shop_tab_chess_ids;
            for (int i = 0; i < tabs_trans.childCount; ++i) {
                Destroy(tabs_trans.GetChild(i).gameObject);
            }
            for (int i = 0; i < chess_ids.Length; ++i) {
                ShopChessTab chess_tab = FM.LoadShopChessTab(GetRandomChess());
                chess_tab.transform.SetParent(tabs_trans);
                chess_tab.rect_trans.localScale = Vector3.one;
            }
            EM.refresh_shop.Invoke();
        }
    }
    public XChess GetRandomChess() {
        var ramdon_num = UnityEngine.Random.Range(0f, 100f);
        float tmp_sum = 0;
        int chess_rarity = 0;
        for (int i = 1; i <= GameInfo.MAX_RARITY; ++i) {
            tmp_sum += GameInfo.shop_buy_chess_probability[GameInfo.exp, i];
            if (ramdon_num <= tmp_sum) {
                chess_rarity = i;
                break;
            }
        }
        var ramdon_id = UnityEngine.Random.Range(0, chess_rarity_dict[chess_rarity].Count);
        return chess_rarity_dict[chess_rarity][ramdon_id];
    }
    public void Activate(int interactable_code = 0) {
        if ((interactable_code == 0 && !is_activate) || (interactable_code == 1)) {
            shop_panel.SetActive(true);
            shop_button_image.sprite = FM.GetShopButtonSprite(true);
            is_activate = true;
        }
        else {
            shop_panel.SetActive(false);
            shop_button_image.sprite = FM.GetShopButtonSprite(false);
            is_activate = false;
        }
    }
    private void UpdateInfo() {
        shop_button_coin_text.text = GameInfo.coin.ToString();
        if (GameInfo.coin >= GameInfo.shop_refresh_coin) {
            refresh_button_coin_text.color = new Color(1, 1, 1);
            refresh_button.interactable = true;
        }
        else {
            refresh_button_coin_text.color = new Color(1, 0.3f, 0.3f);
            refresh_button.interactable = false;
        }
        exp_button.interactable = true;
        if (GameInfo.exp < GameInfo.MAX_EXP) {
            exp_button_coin_text.text = GameInfo.exp_level_up_coin.ToString();
        }
        else {
            exp_button_coin_text.text = "+∞";
            exp_button.interactable = false;
        }
        if (GameInfo.coin >= GameInfo.exp_level_up_coin) {
            exp_button_coin_text.color = new Color(0.82f, 0.67f, 0);
        }
        else {
            exp_button_coin_text.color = new Color(1, 0.3f, 0.3f);
            exp_button.interactable = false;
        }
        exp_button_level_text.text = GameInfo.exp.ToString();
    }
    private void Update() {
        UpdateInfo();
    }
    public void BuyExp() {
        if (GameInfo.exp < GameInfo.MAX_EXP && GameInfo.coin >= GameInfo.exp_level_up_coin) {
            GameInfo.coin -= GameInfo.exp_level_up_coin;
            GameInfo.exp += 1;
        }
    }

}
