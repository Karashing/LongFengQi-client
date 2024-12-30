using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;

public enum GameResult {
    None = 0,
    Win = 1,
    Lose = 2,
    Draw = 3,
}


[Serializable]
public class GameInfo : BaseGame {
    /// <summary>
    /// 
    /// 游戏结果
    /// 
    /// 数据统计
    /// 里程/回合
    /// 阵营
    /// 棋盘
    /// 
    /// 能量点
    /// 金币
    /// 商店
    /// 
    /// XActor
    /// 棋子
    /// 格子
    /// 功能性Actor
    /// 
    /// </summary>

    public void Init() {
        _coin = 0;
        _cur_round = 0;
        _cur_act_time = 0;
        chesses = new List<XChess>();
        grids = new List<XGrid>();
        grid_dict = new Dictionary<Vector3Int, XGrid>();
        special_actors = new List<XActor>();
    }
    public void Update() {
        cur_real_round_time -= Time.deltaTime;
    }


    // ----- 游戏结果 -----
    public GameResult game_result = GameResult.None;
    public float GetGameScore() {
        if (game_result == GameResult.Win) return 1;
        if (game_result == GameResult.Lose) return 0;
        return 0.5f;
    }
    public bool IsGameOver() {
        int self_gong_grid = 0;
        int enemy_gong_grid = 0;
        foreach (var xgrid in GameInfo.grids) {
            if (xgrid.type == GridType.GONG) {
                if (xgrid.camp == XCamp.SELF) self_gong_grid++;
                else if (xgrid.camp == XCamp.ENEMY) enemy_gong_grid++;
            }
        }
        if (self_gong_grid == 0 || enemy_gong_grid == 0) {
            if (self_gong_grid == 0 && enemy_gong_grid == 0) {
                game_result = GameResult.Draw;
            }
            else if (self_gong_grid == 0) {
                game_result = GameResult.Lose;
            }
            else if (enemy_gong_grid == 0) {
                game_result = GameResult.Win;
            }
            return true;
        }
        return false;
    }
    // ----- 数据统计 -----
    public int hit_num = 0;

    // ----- 里程/回合 -----
    public float per_round_time = 30f;
    public XAction cur_action;
    private float _cur_act_time; // 当前行动时刻
    public float cur_act_time {
        get { return _cur_act_time; }
    }
    public void SetCurActTime(float act_time) {
        if (cur_act_time < act_time) {
            _cur_act_time = act_time;
            EM.act_time_change.Invoke();
        }
    }
    private int _cur_round;
    public int cur_round {
        get { return _cur_round; }
    }
    private int _cur_action_id;
    public int cur_action_id {
        get { return _cur_action_id; }
    }
    public XAction NextAction(int round = -1, int action_id = -1) {
        bool is_switch_round = round > _cur_round && action_id == 0;
        _cur_round = round == -1 ? _cur_round + 1 : round;
        _cur_action_id = action_id == -1 ? _cur_action_id + 1 : action_id;
        var action = GM.action_queue.NextAction();
        if (is_switch_round) {
            var last_act_time = cur_act_time;
            SetCurActTime(action.actor.act_time);
            // var delta_coin = (int)(coin_rate * ((int)(cur_act_time / 100) - (int)(last_act_time / 100)) + 1);
            // coin += delta_coin;
            // Debug.Log("coin +" + delta_coin);
            EM.round_change.Invoke();
        }
        return action;
    }
    private float _cur_real_round_time;
    private UnityAction end_round_callback;
    private Dictionary<int, UnityAction> round_time_callbacks = new Dictionary<int, UnityAction>();
    public int cur_round_time { // 当前回合剩余时间(int) 正
        get {
            return Mathf.Max((int)_cur_real_round_time, 0);
        }
    }
    public float cur_real_round_time { // 当前回合剩余时间(float)
        get {
            return _cur_real_round_time;
        }
        set {
            float last_real_round_time = cur_real_round_time;
            _cur_real_round_time = value;
            if (Mathf.Max((int)value, 0) != Mathf.Max((int)last_real_round_time, 0)) {
                EM.round_time_change.Invoke();
            }

            if ((int)value != (int)last_real_round_time) {
                RoundTimeCallback((int)value);
            }

            if (_cur_real_round_time <= 0f && end_round_callback != null) {
                var _call_back = end_round_callback;
                end_round_callback = null;
                _call_back();
            }
        }
    }
    private void RoundTimeCallback(int round_time) {
        if (round_time_callbacks.ContainsKey(round_time)) {
            var _call_back = round_time_callbacks[round_time];
            round_time_callbacks.Remove(round_time);
            _call_back();
        }
    }
    public void SetRoundTimeCallback(int round_time, UnityAction callback) {
        if (round_time_callbacks.ContainsKey(round_time)) {
            round_time_callbacks.Remove(round_time);
        }
        round_time_callbacks.Add(round_time, callback);
    }
    public void ClearRoundTimeCallback() {
        round_time_callbacks.Clear();
    }
    public void SetRoundTime(float time) {
        cur_real_round_time = time;
    }

    // ----- 阵营 -----
    public int server_camp;
    public int GetRealCamp(XCamp xcamp) {
        if (xcamp == XCamp.PUBLIC_ENEMY) return 3;
        if (xcamp == XCamp.NEUTRAL) return 2;
        if (xcamp == XCamp.SELF) return server_camp;
        if (xcamp == XCamp.ENEMY) return 1 - server_camp;
        return -1;
    }
    public XCamp GetLocalCamp(int xcamp) {
        if (xcamp == 3) return XCamp.PUBLIC_ENEMY;
        if (xcamp == 2) return XCamp.NEUTRAL;
        if (xcamp == server_camp) return XCamp.SELF;
        else return XCamp.ENEMY;
    }

    // ----- 棋盘 -----
    public ChessBoardData chess_board;
    public void LoadBoardData(ChessBoardData data) {
        chess_board = data;
        server_camp = chess_board.board_id;
        Debug.Log("Load shops: " + chess_board.shops.Count);
        foreach (var shop_data in chess_board.shops) {
            FM.LoadShop(shop_data);
        }
        // foreach (var chess_data in chess_board.chesses) {
        //     FM.LoadChess(chess_data);
        // }
        Debug.Log("Load grids: " + chess_board.grids.Count);
        foreach (var grid_data in chess_board.grids) {
            FM.LoadGrid(grid_data);
        }
        Debug.Log("Load tian_actors: " + chess_board.tian_actors.Count);
        foreach (var _data in chess_board.tian_actors) {
            FM.LoadTianActor(_data);
        }
    }
    public Vector3Int GetRelativePosition(Vector3Int xpos) {
        if (xpos.y % 2 == 0) {
            return new(1 - xpos.x, -xpos.y);
        }
        else {
            return new(-xpos.x, -xpos.y);
        }
    }
    public Vector3Int GetRealPosition(Vector3Int xpos) {
        if (server_camp == 1) {
            return GetRelativePosition(xpos);
        }
        else {
            return xpos;
        }
    }
    public Vector3Int GetLocalPosition(Vector3Int xpos) {
        if (server_camp == 1) {
            return GetRelativePosition(xpos);
        }
        else {
            return xpos;
        }
    }

    // ----- 经验 -----
    public const int MIN_EXP = 1, MAX_EXP = 7, INF = 1000000000;
    private int _exp;
    public int exp {
        get {
            return _exp;
        }
        set {
            if (_exp != value) {
                _exp = value;
                EM.exp_change.Invoke();
            }
        }
    }
    public int can_inboard_chess_num {
        get {
            return 2 + (exp - 1) / 2;
        }
    }
    private int[] exp_level_up_coins = new int[10]{
        -1,10,25,40,55,70,85,INF,INF,INF
    };
    public int exp_level_up_coin {
        get {
            return Mathf.Max(0, exp_level_up_coins[exp] - (int)(cur_act_time / 50f) * 2);
        }
    }

    // ----- 商店 -----
    public int[] shop_tab_chess_ids = new int[5]; // 商店展示的chess_id
    public int shop_refresh_coin { // 刷新商店所需的花费
        get {
            return Mathf.Min(shop_refresh_times, 2);
        }
    }
    public int shop_refresh_times; // 刷新商店的次数
    public const int MAX_RARITY = 5;
    public float[,] shop_buy_chess_probability = new float[10, MAX_RARITY + 1]{
        {0,100,0,0,0,0},
        {0,50,35,15,0,0},
        {0,40,35,18,7,0},
        {0,30,35,25,10,0},
        {0,29,30,25,15,1},
        {0,25,30,25,17,3},
        {0,19,30,25,20,6},
        {0,15,25,25,25,10},
        {0,10,22,26,27,15},
        {0,100,0,0,0,0},
    };

    // ----- 能量点 -----
    public int max_bean = 5;
    [SerializeField]
    private int _bean;
    public int bean {
        get {
            return _bean;
        }
        set {
            if (_bean != value) {
                _bean = value;
                EM.bean_change.Invoke();
            }
        }
    }
    // ----- 金币 -----
    [SerializeField]
    private int _coin;
    private int[] _secret_coin = new int[100];
    public int coin {
        get {
            return _coin;
        }
        set {
            if (_coin != value) {
                _coin = value;
                EM.coin_change.Invoke();
            }
        }
    }
    private float coin_rate {
        get {
            return cur_act_time / 200f;
        }
    }

    // ----- XActor -----
    public Dictionary<string, XActor> actor_dict = new Dictionary<string, XActor>();
    public XActor cur_act_actor;
    public XActor cur_interact_actor;
    public void AddActor(XActor actor) {
        if (actor is XChess xchess) {
            chesses.Add(xchess);
        }
        else if (actor is XGrid xgrid) {
            grids.Add(xgrid);
            grid_dict.Add(xgrid.grid_position, xgrid);
        }
        else if (actor is XActor xactor) {
            special_actors.Add(xactor);
        }
        actor_dict.Add(actor.server_id, actor);
        EM.actors_change.Invoke();
    }
    public void RemoveActor(XActor actor) {
        if (actor is XChess xchess) {
            chesses.Remove(xchess);
        }
        else if (actor is XGrid xgrid) {
            grids.Remove(xgrid);
            grid_dict.Remove(xgrid.grid_position);
        }
        else if (actor is XActor xactor) {
            special_actors.Remove(xactor);
        }
        actor_dict.Remove(actor.server_id);
        EM.actors_change.Invoke();
    }
    // ----- 棋子 -----
    public List<XChess> chesses = new List<XChess>();
    public int chess_num {
        get {
            return chesses.Count;
        }
    }
    public int self_in_board_countable_chess_num {
        get {
            int _chess_num = 0;
            foreach (XChess chess in chesses) {
                if (chess.grid.in_board && chess.camp == XCamp.SELF && chess.can_buy) _chess_num++;
            }
            return _chess_num;
        }
    }
    public int self_in_board_chess_num {
        get {
            int _chess_num = 0;
            foreach (XChess chess in chesses) {
                if (chess.grid.in_board && chess.camp == XCamp.SELF) _chess_num++;
            }
            return _chess_num;
        }
    }
    public int self_prepare_chess_num {
        get {
            int _chess_num = 0;
            foreach (XChess chess in chesses) {
                if (chess.grid.in_prepare && chess.camp == XCamp.SELF) _chess_num++;
            }
            return _chess_num;
        }
    }
    public List<XChess> GetChesss(XCamp xcamp) {
        List<XChess> _chesses = new List<XChess>();
        foreach (XChess chess in chesses) {
            if (chess.camp == xcamp) _chesses.Add(chess);
        }
        return _chesses;
    }
    public List<XChess> GetChesss(ChessType chess_type, XCamp xcamp) {
        List<XChess> _chesses = new List<XChess>();
        foreach (XChess chess in chesses) {
            if (chess.type == chess_type && chess.camp == xcamp) _chesses.Add(chess);
        }
        return _chesses;
    }
    public List<XChess> GetSelfPrepareChesss(ChessType chess_type) {
        List<XChess> _chesses = GetChesss(chess_type, XCamp.SELF);
        List<XChess> __chesses = new List<XChess>();
        foreach (XChess chess in _chesses) {
            if (chess.grid.type == GridType.prepare_chess) __chesses.Add(chess);
        }
        return __chesses;
    }
    public List<XChess> GetSelfInBoardChesses() {
        List<XChess> _chesses = GetChesss(XCamp.SELF);
        List<XChess> __chesses = new List<XChess>();
        foreach (XChess chess in _chesses) {
            if (chess.grid.in_board) __chesses.Add(chess);
        }
        return __chesses;
    }

    // ----- 格子 -----
    public List<XGrid> grids = new List<XGrid>();
    public Dictionary<Vector3Int, XGrid> grid_dict = new Dictionary<Vector3Int, XGrid>();
    public int self_prepare_grid_num {
        get {
            int _prepare_grid_num = 0;
            foreach (var grid in grids) {
                if (grid.type == GridType.prepare_chess && grid.camp == XCamp.SELF) _prepare_grid_num++;
            }
            return _prepare_grid_num;
        }
    }
    public List<XGrid> self_prepare_grids {
        get {
            List<XGrid> _prepare_grids = new List<XGrid>();
            foreach (XGrid grid in grids) {
                if (grid.type == GridType.prepare_chess && grid.camp == XCamp.SELF) _prepare_grids.Add(grid);
            }
            return _prepare_grids;
        }
    }
    public List<XGrid> GetGrids(GridType grid_type, XCamp xcamp) {
        List<XGrid> _grids = new List<XGrid>();
        foreach (XGrid grid in grids) {
            if (grid.type == grid_type && grid.camp == xcamp) _grids.Add(grid);
        }
        return _grids;
    }
    public List<XGrid> GetGrids(GridType grid_type) {
        List<XGrid> _grids = new List<XGrid>();
        foreach (XGrid grid in grids) {
            if (grid.type == grid_type) _grids.Add(grid);
        }
        return _grids;
    }
    public List<XGrid> GetSelfBuildingGrids() {
        List<XGrid> _grids = new List<XGrid>();
        foreach (XGrid grid in grids) {
            if (grid.in_building && grid.camp == XCamp.SELF) _grids.Add(grid);
        }
        return _grids;
    }

    // ----- 功能性Actor -----
    public List<XActor> special_actors = new List<XActor>();
    public bool HaveTianActor() {
        foreach (var xactor in special_actors) {
            if (xactor is TianActor) return true;
        }
        return false;
    }
}