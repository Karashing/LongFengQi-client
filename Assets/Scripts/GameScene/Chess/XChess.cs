using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;
using UnityEngine.Networking.Types;
using UnityEngine.Events;

public class XChess : XHpActor {
    public TMP_Text shield_text;
    public GameObject shield_go;
    public TMP_Text hp_text;
    public SpriteRenderer chess_bg_spr;
    public Vector3 chess_size {
        get {
            return chess_bg_spr.bounds.size;
        }
    }
    public TMP_Text chess_word_text;
    public TMP_Text level_text;
    public SpriteRenderer level_bar_bg_spr, level_bar_fill_spr;

    /// <summary>
    /// UI相关
    /// </summary>

    public float chess_tab_detail_height;

    /// <summary>
    /// 固定数据 | 类型相关
    /// </summary>
    public ChessType type; // 棋子类型
    public bool can_buy = true;
    public string career; // 命途
    public int rarity; // 稀有度
    // describe

    /// <summary>
    /// 游戏中实时数据
    /// </summary>
    protected int prepare_chess_num_for_level;
    public int need_chess_num_for_level {
        get {
            if (level == 0) return 2;
            else return 4;
        }
    }
    // [SerializeField]
    // private int _max_hp;
    // private int _max_hp_by_level {
    //     get {
    //         return (int)(Mathf.Pow(1.5f, level) * _max_hp);
    //     }
    // }
    // public int max_hp {
    //     get { return _max_hp_by_level; }
    // }
    protected override void UpdateHP() {
        base.UpdateHP();
        hp_text.text = hp.ToString();
    }
    [SerializeField]
    private int _attack;
    private int _attack_by_level {
        get {
            return (int)(Mathf.Pow(1.5f, level) * _attack);
        }
    }
    public int attack {
        get { return _attack_by_level; }
    }
    public int cur_attack {
        get {
            var real_attack = attack;
            foreach (var attack_buff in GetBuffs<IAttackBuff>()) {
                real_attack += attack_buff.GetDeltaAttack();
            }
            real_attack = Mathf.Max(real_attack, 0);
            return real_attack;
        }
    }
    private XGrid _grid = null;
    public XGrid grid {
        get { return _grid; }
    }

    /// <summary>
    /// END
    /// </summary>

    protected override void Awake() {
        base.Awake();
        functional_skills = new List<XSkill> {
            // new XLevelupSkill(this, 1000),
        };
    }

    public void Init(ChessData xchess) { // TODO: 区分我方/敌方
        server_id = xchess.server_id;
        XGrid xgrid = null;
        if (xchess.position_type == ChessPositionType.random_prepare_grid) {
            foreach (XGrid grid in GameInfo.GetGrids(GridType.prepare_chess, xchess.local_camp)) {
                if (grid.state == GridState.EMPTY) {
                    xgrid = grid;
                    break;
                }
            }
        }
        else if (xchess.position_type == ChessPositionType.grid_server_id) {
            xgrid = (XGrid)GameInfo.actor_dict[xchess.position_info];
        }
        else {
            Debug.LogError($"word: {word} xchess.position_type == ChessPositionType.none");
            return;
        }

        shield_go?.SetActive(false);
        level = xchess.level;
        camp = xchess.local_camp;
        prepare_chess_num_for_level = 0;
        hp = max_hp;
        chess_word_text.color = FM.GetCampColor(camp);
        MoveToGrid(xgrid, 0);
        GameInfo.AddActor(this);
        Debug.Log(word + "Init!");
    }

    private TipSelect tip_select;
    public override void ActInteractStart(List<XSkill> xskills, bool is_auto = false) {
        bool can_action = IsCanActByBuffs();
        if (!is_auto && can_action) {
            GM.interact_queue.Init(this, xskills);
            tip_select = FM.LoadTipSelect(this);
        }
        GM.camera_moudle.FocusOnPosition(transform.position);
        base.ActInteractStart(xskills);
        if (!can_action) {
            ActInteractEnd();
            XSkill.ExecuteNoneSkill(server_id, act_round);
        }
    }

    public override void ActInteractEnd() {
        GM.interact_queue.EndInteract();
        if (tip_select != null) {
            tip_select.End();
            tip_select = null;
        }
        base.ActInteractEnd();
    }

    public bool CanEnterActionQueue() {
        if (hp <= 0) return false;
        return true;
    }
    public void MoveToGrid(XGrid xgrid, float duration = 0.5f) {
        UnityAction mid_callback = null;
        if (xgrid == null) {
            Debug.LogError($"move: {word} -> null");
        }
        else {
            Debug.Log($"move: {word} -> {xgrid.word}({xgrid.grid_position})");
        }
        if (xgrid != null && xgrid.state == GridState.HAVING) {
            var ygrid = grid;
            var xchess = xgrid.bind_chess;
            mid_callback = () => {
                xchess.MoveToGrid(ygrid, duration);
            };
        }
        if ((grid == null || !grid.in_board) && (xgrid != null && xgrid.in_board)) {
            ChangeGrid(xgrid, mid_callback);
            if (CanEnterActionQueue()) EnterActionQueue(0.5f);
        }
        else if ((xgrid == null || !xgrid.in_board) && (grid != null && grid.in_board)) {
            ChangeGrid(xgrid, mid_callback);
            QuitActionQueue();
        }
        else {
            ChangeGrid(xgrid, mid_callback);
        }
        if (duration <= 0) transform.position = GM.grid_map.GetCellCenterWorld(xgrid);
        else transform.DOMove(GM.grid_map.GetCellCenterWorld(xgrid), duration).SetEase(Ease.InQuad);
    }

    public void ChangeGrid(XGrid grid) {
        if (_grid != null) {
            _grid.UnbindChess();
        }
        _grid = grid;
        if (_grid != null) {
            _grid.BindChess(this);
            if (grid.IsVisible()) {
                gameObject.SetActive(true);
            }
            else {
                gameObject.SetActive(false);
            }
        }
    }

    private void ChangeGrid(XGrid grid, UnityAction mid_callback) { // 改变Grid
        if (_grid != null) {
            _grid.UnbindChess();
        }
        _grid = null;
        if (mid_callback != null) mid_callback();
        _grid = grid;
        if (_grid != null) {
            _grid.BindChess(this);
            if (grid.IsVisible()) {
                gameObject.SetActive(true);
            }
            else {
                gameObject.SetActive(false);
            }
        }
    }

    public override void RefreshBuffEffect() {
        // shield
        Debug.Log("cal shield.Count = " + GetBuffs<IShieldBuff>().Count);
        int total_shield = 0;
        foreach (var shield_buff in GetBuffs<IShieldBuff>()) {
            total_shield += shield_buff.GetShield(this);
        }
        if (total_shield > 0) {
            shield_text.text = total_shield.ToString();
            shield_go.SetActive(true);
        }
        else {
            shield_go.SetActive(false);
        }

        // speed
        base.RefreshBuffEffect();
    }

    protected List<GameObject> level_effects = new List<GameObject>();
    public override void UpdateLevel(int delta_level) {
        while (level_effects.Count > level) {
            Destroy(level_effects[^1]);
            level_effects.RemoveAt(level_effects.Count - 1);
        }
        if (level_effects.Count < level) {
            FM.LoadChessLevelEffect(this);
        }

        hp = (int)(Mathf.Pow(1.5f, delta_level) * hp);
        level_text.text = $"Lv.{perform_level}";
        UpdatePrepareChessNumForLevel();
    }
    public void UpdatePrepareChessNumForLevel() {
        var angle = Mathf.Clamp(22.5f * need_chess_num_for_level, 0, 360);
        if (level_bar_bg_spr != null)
            level_bar_bg_spr.material.SetFloat("_Angle", angle);

        angle = Mathf.Clamp(22.5f * (level == max_level ? need_chess_num_for_level : prepare_chess_num_for_level), 0, 360);
        if (level_bar_fill_spr != null) {
            level_bar_fill_spr.material.SetFloat("_Angle", angle);

            if (level == max_level) {
                level_bar_fill_spr.color = new Color(255, 215, 0);
            }
            else {
                level_bar_fill_spr.color = new Color(255, 255, 255);
            }
        }
    }
    public void MergeChessToLevelUp() {
        prepare_chess_num_for_level += 1;
        void UpdateChessToLevelUp() {
            if (prepare_chess_num_for_level >= need_chess_num_for_level && level < max_level) {
                prepare_chess_num_for_level -= need_chess_num_for_level;
                level += 1;
                if (grid.IsVisible()) {
                    var effect = FM.LoadEffect("chess_level_up_effect");
                    effect.transform.position = transform.position;
                    effect.Play();
                }
            }
            UpdatePrepareChessNumForLevel();
        }
        if (level_bar_fill_spr != null) {
            DOTween.To(() => level_bar_fill_spr.material.GetFloat("_Angle"),
                       (x) => level_bar_fill_spr.material.SetFloat("_Angle", x),
                       Mathf.Clamp(22.5f * prepare_chess_num_for_level, 0, 360),
                       0.5f).OnComplete(UpdateChessToLevelUp);
        }
        else {
            UpdateChessToLevelUp();
        }
    }

    public virtual bool CanBeTarget(XCamp xcamp) {
        if (camp != xcamp) return false;
        if (hp <= 0) return false;
        return true;
    }
    public virtual bool CanBeTarget(XTarget target, params XCamp[] able_camps) {
        if (hp <= 0) return false;
        if (target == XTarget.ANY || target == XTarget.CHESS) {
            if (able_camps.Contains(camp)) return true;
        }
        return false;
    }




    public virtual void BeRestore(int inhp, UnityAction after_callback = null) {
        hp = Mathf.Min(hp + inhp, max_hp);
        var hp_effect = FM.LoadHpIncreaseEffect(this, inhp);
    }
    public override void BeAttack(int atk, XActor source_actor, AttackType atk_type = AttackType.NORMAL, UnityAction after_callback = null) {
        base.BeAttack(atk, source_actor, atk_type, after_callback);
        Sequence seq0 = DOTween.Sequence();
        seq0.Append(DOTween.To(() => chess_bg_spr.color,
                            x => chess_bg_spr.color = x,
                            new Color(0.75f, 0.25f, 0.25f, 1), 0.2f).SetEase(Ease.OutQuad));
        seq0.Append(DOTween.To(() => chess_bg_spr.color,
                            x => chess_bg_spr.color = x,
                            Color.white, 0.5f).SetEase(Ease.InQuad));
    }

    // public virtual void BeAttack(int atk, XActor source_actor, bool is_end = true, bool can_counter_atk = true, UnityAction after_callback = null) {
    //     var tmp_atk = atk;
    //     foreach (var be_attack_buff in GetBuffs<IBeAttackBuff>()) {
    //         tmp_atk += be_attack_buff.GetDeltaBeAttack();
    //     }
    //     atk = Mathf.Max(tmp_atk, 0);
    //     foreach (var shield_buff in GetBuffs<IShieldBuff>()) {
    //         atk = shield_buff.BeAttack(atk, this);
    //     }
    //     var counter_atk = 0;
    //     foreach (var counter_attack_buff in GetBuffs<ICounterAttackBuff>()) {
    //         counter_atk = counter_attack_buff.GetCounterAttack(Mathf.Min(atk, hp), source_actor, this);
    //     }

    //     hp = Mathf.Max(hp - atk, 0);
    //     if (can_counter_atk) {
    //         if (source_actor is XChess source_chess) {
    //             if (counter_atk > 0)
    //                 source_chess.BeAttack(counter_atk, this, can_counter_atk: false);
    //         }
    //     }

    //     RefreshBuffEffect();
    //     Sequence seq0 = DOTween.Sequence();
    //     seq0.Append(DOTween.To(() => chess_bg_spr.color,
    //                         x => chess_bg_spr.color = x,
    //                         new Color(0.75f, 0.25f, 0.25f, 1), 0.2f).SetEase(Ease.OutQuad));
    //     seq0.Append(DOTween.To(() => chess_bg_spr.color,
    //                         x => chess_bg_spr.color = x,
    //                         Color.white, 0.5f).SetEase(Ease.InQuad));
    //     var hp_effect = FM.LoadHpDecreaseEffect(this, atk);

    //     // if(hp==0)
    //     if (is_end) {
    //         Sequence seq1 = DOTween.Sequence();
    //         seq1.AppendInterval(0.45f);
    //         seq1.AppendCallback(() => {
    //             if (hp == 0) {
    //                 BeInjured();
    //             }
    //             if (after_callback != null) {
    //                 after_callback();
    //             }
    //         });
    //     }
    // }

    protected override void BeInjured(XActor source_actor) {
        base.BeInjured(source_actor);
        QuitActionQueue();
        foreach (var xgrid in GameInfo.GetGrids(GridType.CHENG, camp)) {
            if (xgrid.state == GridState.EMPTY) {
                ChangeGrid(xgrid);
                transform.DOMove(GM.grid_map.GetCellCenterWorld(xgrid), 1f).SetEase(Ease.InQuad);
                return;
            }
        }
        // foreach (var xgrid in GameInfo.GetGrids(GridType.GONG, camp)) {
        //     if (xgrid.state == GridState.EMPTY) {
        //         ChangeGrid(xgrid);
        //         transform.DOMove(GM.grid_map.GetCellCenterWorld(xgrid), 1f).SetEase(Ease.InQuad);
        //         return;
        //     }
        // }
        Kill();
    }

    public void TryKill() {
        NM.destroy_actor.Send(new ActorData(new ChessData(
            serverId: server_id,
            chessType: type,
            chessLevel: level,
            xcamp: camp,
            chessPositionType: ChessPositionType.none
        )));
    }
    public override void Kill() {
        QuitActionQueue();
        ChangeGrid(null);
        GameInfo.RemoveActor(this);
        DestroyGameObject();
    }





    int GetChessCoin() {
        if (level == 0) return rarity * (prepare_chess_num_for_level + 1);
        else if (level == 1) return rarity * (prepare_chess_num_for_level + 3) - 1;
        else return rarity * 6;
    }

    private Vector3 offset;
    private bool isMoving = false, canMove = true;
    private CloneChess clone_chess;
    private void OnMouseDown() { //鼠标按下
        if (grid.in_prepare && canMove) {
            var xgrid = GameInfo.GetGrids(GridType.delete_chess, XCamp.SELF)[0] as DeleteChessGrid;
            xgrid.Show(GetChessCoin());
            offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clone_chess = FM.LoadCloneChess(this);
            clone_chess.SetPosition(transform.position);
            GM.camera_moudle.ban_move++;
            isMoving = true;
        }
    }
    private void OnMouseUp() {
        if (isMoving && clone_chess != null) {
            GM.camera_moudle.ban_move--;
            isMoving = false;
            // TODO
            var delete_grid = GameInfo.GetGrids(GridType.delete_chess, XCamp.SELF)[0] as DeleteChessGrid;
            var last_position = clone_chess.transform.position;
            Debug.Log(Vector3.Distance(GM.grid_map.GetCellCenterWorld(delete_grid), last_position));
            clone_chess.Kill();
            delete_grid.Hide();
            if (Vector3.Distance(GM.grid_map.GetCellCenterWorld(delete_grid), last_position) <= chess_size.x * 0.4f) {
                // 售出
                GameInfo.coin += GetChessCoin();
                TryKill();
            }
            else {
                foreach (var xgrid in GameInfo.self_prepare_grids) {
                    if (Vector3.Distance(GM.grid_map.GetCellCenterWorld(xgrid), last_position) <= chess_size.x * 0.5f) {
                        MoveToGrid(xgrid, 0.2f);
                        break;
                    }
                }
            }
        }
        clone_chess = null;
    }
    void OnMouseDrag() { //鼠标持续按下
        if (grid.in_prepare && isMoving && clone_chess != null) {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            clone_chess.SetPosition(new Vector3(newPosition.x, newPosition.y, transform.position.z));
        }
    }
    // void OnMouseUpAsButton() { }
    // private void OnCollisionEnter2D(Collision2D other) { }
}