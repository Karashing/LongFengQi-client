using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using ToolI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FactoryManager : BaseBehaviour {
    private static FactoryManager _instance;
    public static FactoryManager Ins {
        get {
            return _instance;
        }
    }
    void Awake() {
        _instance = this;
    }

    public Sprite[] chess_boards; // 棋盘
    public Sprite GetChessBoard(int board_code) {
        return chess_boards[board_code];
    }

    // [SerializeField]
    // private Sprite[][] chess_word_sprites; // 棋的文字
    // public Sprite GetChessWord(XChess chess) {
    //     return chess_word_sprites[chess.id][(int)chess.camp];
    // }

    public XConversation LoadXConversation(ConversationData data, bool is_right = true) {
        GameObject _prefab;
        if (is_right) _prefab = Resources.Load<GameObject>("Prefabs/right_conversation");
        else _prefab = Resources.Load<GameObject>("Prefabs/left_conversation");
        GameObject _go = Instantiate(_prefab);
        XConversation _x = _go.GetComponent<XConversation>();
        _x.Init(data);
        return _x;
    }
    public XEmotion LoadXEmotion(int emotion_id, bool is_right = true) {
        GameObject _prefab = Resources.Load<GameObject>("Prefabs/Emotion/emotion_" + emotion_id.ToString());
        GameObject _go = Instantiate(_prefab);
        XEmotion _x = _go.GetComponent<XEmotion>();
        _x.Init(is_right);
        return _x;
    }
    public EmotionUI LoadEmotionUI(int emotion_id) {
        GameObject _prefab = Resources.Load<GameObject>("Prefabs/Emotion/emotion_ui");
        GameObject _go = Instantiate(_prefab);
        EmotionUI _x = _go.GetComponent<EmotionUI>();
        _x.Init(emotion_id);
        return _x;
    }
    public Sprite GetEmotionSprite(int emotion_id) {
        var path = "Image/Emotion/emotion_" + emotion_id.ToString();
        return Resources.Load<Sprite>(path);
    }

    [SerializeField]
    private Sprite[] action_card_bgs; // 行动卡背景
    public Sprite GetActionCardBg(XActor actor) {
        return action_card_bgs[(int)actor.camp];
    }
    public Sprite GetXActionCardBg(XActionCard card) {
        if (card is ActionCard) {
            return action_card_bgs[(int)card.action.actor.camp];
        }
        else if (card is ExtraActionCard) {
            return extra_action_card_small_bgs[(int)card.action.actor.camp];
        }
        return null;
    }
    [SerializeField]
    private Sprite[] extra_action_card_small_bgs; // 行动卡背景
    public Sprite GetExtraActionCardSmallBg(XActor actor) {
        return extra_action_card_small_bgs[(int)actor.camp];
    }
    [SerializeField]
    private Sprite[] extra_action_card_bgs; // 行动卡背景
    public Sprite GetExtraActionCardBg(XActor actor) {
        return extra_action_card_bgs[(int)actor.camp];
    }

    [SerializeField]
    private Color[] camp_colors;
    public Color GetCampColor(XCamp xcamp) {
        return camp_colors[(int)xcamp];
    }
    public string GetCampColorHex(XCamp xcamp) {
        return ColorI.ToHexString(camp_colors[(int)xcamp]);
    }


    public ActionCard LoadActionCard(XAction xaction) {
        GameObject action_card_prefab = Resources.Load<GameObject>("Prefabs/action_card");
        GameObject action_card_go = Instantiate(action_card_prefab);
        ActionCard action_card = action_card_go.GetComponent<ActionCard>();
        action_card.Init(xaction);
        return action_card;
    }
    public ExtraActionCard LoadExtraActionCard(XAction xaction) {
        GameObject action_card_prefab = Resources.Load<GameObject>("Prefabs/extra_action_card");
        GameObject action_card_go = Instantiate(action_card_prefab);
        ExtraActionCard action_card = action_card_go.GetComponent<ExtraActionCard>();
        action_card.Init(xaction);
        return action_card;
    }


    // [SerializeField]
    // private string chess_board_data_json_name;
    public ChessBoardData LoadChessBoardData(int board_id) {
        return JsonI.ReadFromJson<ChessBoardData>("ChessBoard/board_" + board_id.ToString() + ".json");
    }

    [SerializeField]
    private Sprite[] shop_buttons;
    public Sprite GetShopButtonSprite(bool is_activate) {
        if (is_activate) return shop_buttons[0];
        else return shop_buttons[1];
    }
    public Sprite GetSymbolSprite(SymbolType symbol_type) {
        var path = "Image/ChessSymbol/chess_symbol_" + ((int)symbol_type).ToString();
        Debug.Log(path);
        return Resources.Load<Sprite>(path);
    }
    public Sprite GetIconSprite(string icon_name) {
        return Resources.Load<Sprite>("Image/Icon/" + icon_name);
    }
    [SerializeField]
    private Sprite[] shop_tab_bgs;
    public Sprite GetShopTabBGSprite(int rarity) {
        return shop_tab_bgs[rarity - 1];
    }

    // [SerializeField]
    // private GameObject shop_chess_tab_prefab;
    public ShopIncomeDetail LoadShopIncomeDetail(int coin, string describe) {
        GameObject _prefab = Resources.Load<GameObject>("Prefabs/income_detail");
        GameObject _go = Instantiate(_prefab);
        ShopIncomeDetail _x = _go.GetComponent<ShopIncomeDetail>();
        _x.Init(coin, describe);
        return _x;
    }
    public ShopChessTab LoadShopChessTab(XChess chess) {
        GameObject shop_chess_tab_prefab = Resources.Load<GameObject>("Prefabs/ShopChessTab/chess_tap_0");
        GameObject chess_tab_go = Instantiate(shop_chess_tab_prefab);
        ShopChessTab chess_tab = chess_tab_go.GetComponent<ShopChessTab>();
        chess_tab.Init(chess);
        return chess_tab;
    }

    public InteractCard LoadInteractCard(XSkill xskill, int interact_mode = 0) {
        GameObject card_prefab;
        if (interact_mode == 0)
            card_prefab = Resources.Load<GameObject>("Prefabs/interact_card");
        else
            card_prefab = Resources.Load<GameObject>("Prefabs/interact_card1");
        GameObject card_go = Instantiate(card_prefab);
        card_go.transform.SetParent(actors_parent_trans);
        InteractCard card = card_go.GetComponent<InteractCard>();
        card.Init(xskill);
        return card;
    }
    public BigSkillCard LoadBigSkillCard(XActor xactor) {
        GameObject card_prefab = Resources.Load<GameObject>("Prefabs/big_skill_card");
        GameObject card_go = Instantiate(card_prefab);
        BigSkillCard card = card_go.GetComponent<BigSkillCard>();
        card.Init(xactor);
        return card;
    }

    public ActorNormalUI LoadNormalActorUI(XActor xactor) {
        GameObject _prefab = Resources.Load<GameObject>("Prefabs/UI/ActorNormalUI");
        GameObject _go = Instantiate(_prefab);
        ActorNormalUI _effect = _go.GetComponent<ActorNormalUI>();
        _effect.Init(xactor);
        return _effect;
    }
    public ActorUI LoadActorUI(XActor xactor) {
        GameObject _prefab = null;
        if (xactor is XChess) {
            _prefab = Resources.Load<GameObject>("Prefabs/UI/ChessUI");
        }
        else if (xactor is XGrid) {
            _prefab = Resources.Load<GameObject>("Prefabs/UI/GridUI");
        }
        else {
            _prefab = Resources.Load<GameObject>("Prefabs/UI/XActorUI");
        }
        GameObject _go = Instantiate(_prefab);
        ActorUI _effect = _go.GetComponent<ActorUI>();
        _effect.Init(xactor);
        return _effect;
    }
    public GameObject LoadChessLevelEffect(XActor xactor) {
        GameObject _prefab = Resources.Load<GameObject>("Prefabs/Effects/chess_level_" + xactor.level.ToString() + "_effect");
        GameObject _go = Instantiate(_prefab);
        _go.transform.SetParent(xactor.transform);
        _go.transform.localPosition = Vector3.zero;
        return _go;
    }
    public HpIncreaseEffect LoadHpIncreaseEffect(XActor xactor, int hp_increase) {
        GameObject _prefab = Resources.Load<GameObject>("Prefabs/Effects/hp_increase_effect");
        GameObject _go = Instantiate(_prefab);
        _go.transform.SetParent(actors_parent_trans);
        HpIncreaseEffect _effect = _go.GetComponent<HpIncreaseEffect>();
        _effect.Init(xactor, hp_increase);
        return _effect;
    }
    public HpDecreaseEffect LoadHpDecreaseEffect(XActor xactor, int hp_decrease) {
        GameObject _prefab = Resources.Load<GameObject>("Prefabs/Effects/hp_decrease_effect");
        GameObject _go = Instantiate(_prefab);
        _go.transform.SetParent(actors_parent_trans);
        HpDecreaseEffect _effect = _go.GetComponent<HpDecreaseEffect>();
        _effect.Init(xactor, hp_decrease);
        return _effect;
    }
    public TipSelect LoadTipSelect(XActor xchess) {
        GameObject tip_prefab = Resources.Load<GameObject>("Prefabs/tip_select");
        GameObject tip_go = Instantiate(tip_prefab);
        tip_go.transform.SetParent(actors_parent_trans);
        TipSelect tip = tip_go.GetComponent<TipSelect>();
        tip.Init(xchess);
        return tip;
    }
    public TipEffect LoadTipEffect(Vector3Int xpos) {
        GameObject tip_prefab = Resources.Load<GameObject>("Prefabs/tip_effect");
        GameObject tip_go = Instantiate(tip_prefab);
        tip_go.transform.SetParent(actors_parent_trans);
        TipEffect tip = tip_go.GetComponent<TipEffect>();
        tip.Init(xpos);
        return tip;
    }
    public SkillExecuteTipEffect LoadSkillExecuteTipEffect(XActor xactor, XSkill xskill) {
        GameObject _prefab = Resources.Load<GameObject>("Prefabs/skill_execute_tip_effect");
        GameObject _go = Instantiate(_prefab);
        SkillExecuteTipEffect effect = _go.GetComponent<SkillExecuteTipEffect>();
        effect.Init(xactor, xskill);
        return effect;
    }
    public BeanCard LoadBean() {
        GameObject _prefab = Resources.Load<GameObject>("Prefabs/bean_card");
        GameObject _go = Instantiate(_prefab);
        BeanCard bean = _go.GetComponent<BeanCard>();
        bean.Init();
        return bean;
    }
    public XEffectAction LoadEffect(string effect_name, UnityAction<int> call_back) {
        GameObject _prefab = Resources.Load<GameObject>("Prefabs/Effects/" + effect_name);
        GameObject _go = Instantiate(_prefab);
        XEffectAction effect = _go.GetComponent<XEffectAction>();
        effect.Init(call_back);
        return effect;
    }
    public XEffectAction LoadEffect(string effect_name, UnityAction call_back = null) {
        GameObject _prefab = Resources.Load<GameObject>("Prefabs/Effects/" + effect_name);
        GameObject _go = Instantiate(_prefab);
        XEffectAction effect = _go.GetComponent<XEffectAction>();
        effect.Init(call_back);
        return effect;
    }
    [SerializeField]
    private Transform actors_parent_trans;
    public CloneChess LoadCloneChess(XChess xchess) {
        GameObject chess_prefab = Resources.Load<GameObject>("Prefabs/clone_chess");
        GameObject chess_go = Instantiate(chess_prefab);
        chess_go.transform.SetParent(actors_parent_trans);
        CloneChess chess = chess_go.GetComponent<CloneChess>();
        chess.Init(xchess);
        return chess;
    }
    public XChess LoadChess(ChessData xchess) {
        GameObject chess_prefab = Resources.Load<GameObject>("Prefabs/Chess/chess_" + ((int)xchess.type).ToString());
        GameObject chess_go = Instantiate(chess_prefab);
        chess_go.transform.SetParent(actors_parent_trans);
        XChess chess = chess_go.GetComponent<XChess>();
        chess.Init(xchess);
        return chess;
    }
    public XGrid LoadGrid(GridData xgrid_data) {
        GameObject grid_prefab = Resources.Load<GameObject>("Prefabs/Grid/grid_" + ((int)xgrid_data.type).ToString());
        GameObject grid_go = Instantiate(grid_prefab);
        grid_go.transform.SetParent(actors_parent_trans);
        XGrid grid = grid_go.GetComponent<XGrid>();
        grid.Init(xgrid_data);
        return grid;
    }
    public ChessShop LoadShop(XActorData xshop_data) {
        try {
            Debug.Log("LoadShop1");
            GameObject shop_prefab = Resources.Load<GameObject>("Prefabs/SpecialActor/shop");
            Debug.Log("LoadShop2");
            GameObject shop_go = Instantiate(shop_prefab);
            Debug.Log("LoadShop3");
            shop_go.transform.SetParent(actors_parent_trans);
            Debug.Log("LoadShop4");
            ChessShop shop = shop_go.GetComponent<ChessShop>();
            Debug.Log("LoadShop5");
            shop.Init(xshop_data);
            Debug.Log("LoadShop6");
            return shop;
        }
        catch (Exception e) {
            Debug.Log("LoadShop Exception: " + e);
            return new ChessShop();
        }
    }
    public TianActor LoadTianActor(XActorData _data) {
        GameObject _prefab = Resources.Load<GameObject>("Prefabs/SpecialActor/tian_actor");
        GameObject _go = Instantiate(_prefab);
        _go.transform.SetParent(actors_parent_trans);
        TianActor xactor = _go.GetComponent<TianActor>();
        xactor.Init(_data);
        return xactor;
    }
    public TaBuffActor LoadTaBuffActor(XActorData _data, TaGrid _grid) {
        GameObject _prefab = Resources.Load<GameObject>("Prefabs/SpecialActor/ta_buff_actor");
        GameObject _go = Instantiate(_prefab);
        _go.transform.SetParent(actors_parent_trans);
        TaBuffActor xactor = _go.GetComponent<TaBuffActor>();
        xactor.Init(_data, _grid);
        return xactor;
    }
}
