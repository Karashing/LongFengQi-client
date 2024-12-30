using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;


public class XGrid : XHpActor {
    public SpriteRenderer sprite_renderer;
    public TMP_Text hp_text;
    public Sprite[] grid_sprites;
    /// <summary>
    /// 固定数据
    /// </summary>
    public GridType type;
    public bool can_bind_chess = true;
    public bool in_board {
        get {
            return type != GridType.prepare_chess && type != GridType.prepare_item && type != GridType.delete_chess && type != GridType.None;
        }
    }
    public bool in_building {
        get {
            return type == GridType.QIANG || type == GridType.LOU || type == GridType.FU || type == GridType.CHENG || type == GridType.GONG;
        }
    }
    public bool in_prepare {
        get {
            return type == GridType.prepare_chess;
        }
    }

    /// <summary>
    /// 游戏中实时数据
    /// </summary>
    // public int max_hp;
    // private int _hp;
    // public int hp {
    //     get { return _hp; }
    //     set {
    //         _hp = value;
    //         hp_text.text = _hp.ToString();
    //     }
    // }
    protected override void UpdateHP() {
        base.UpdateHP();
        hp_text.text = hp.ToString();
    }
    public bool have_hp {
        get { return hp_text != null; }
    }
    [HideInInspector]
    public Vector3Int grid_position;
    [HideInInspector]
    public GridState state = GridState.EMPTY;

    private XChess _bind_chess;
    public XChess bind_chess {
        get { return _bind_chess; }
    }
    public virtual void BindChess(XChess chess) {
        _bind_chess = chess;
        state = GridState.HAVING;
    }
    public virtual void UnbindChess() {
        _bind_chess = null;
        state = GridState.EMPTY;
    }


    public bool IsVisible() {
        if (!in_board) {
            if (camp == XCamp.ENEMY) {
                return false;
            }
        }
        return true;
    }

    public virtual void Init(GridData grid_data) {
        server_id = grid_data.server_id;
        grid_position = grid_data.position;
        level = grid_data.level;
        camp = grid_data.local_camp;
        UnbindChess();
        transform.position = GM.grid_map.GetCellCenterWorld(grid_position);
        if (have_hp) {
            hp = max_hp;
            hp_text.color = FM.GetCampColor(camp);
        }
        if (IsVisible()) {
            gameObject.SetActive(true);
        }
        else {
            gameObject.SetActive(false);
        }
        GameInfo.AddActor(this);
    }

    public override void UpdateLevel(int delta_level) {
        base.UpdateLevel(delta_level);
        sprite_renderer.sprite = grid_sprites[level];
    }

    private TipSelect tip_select;
    public override void ActInteractStart(List<XSkill> xskills, bool is_auto = false) {
        if (!is_auto) {
            tip_select = FM.LoadTipSelect(this);
            GM.interact_queue.Init(this, xskills);
        }
        GM.camera_moudle.FocusOnPosition(transform.position);
        base.ActInteractStart(xskills);
    }

    public override void ActInteractEnd() {
        GM.interact_queue.EndInteract();
        if (tip_select != null) {
            tip_select.End();
            tip_select = null;
        }
        base.ActInteractEnd();
    }

    public bool CanSummon(XChess xchess) { // 召唤
        if (!CanBeTarget()) return false;
        if (camp == xchess.opposite_camp) return false;
        if (state == GridState.HAVING) return false;
        return true;
    }
    public bool CanBeTarget() {
        if (!in_board) return false;
        if (!can_bind_chess) return false;
        return true;
    }
    // public bool CanBeTarget(XCamp xcamp, bool can_select_grid = true) { // (select_grid.camp == xcamp)
    //     if (!in_board) return false;
    //     if (camp != xcamp || (!can_select_grid)) {
    //         if (state != GridState.HAVING) return false;
    //         if (!bind_chess.CanBeTarget(xcamp)) return false;
    //     }
    //     return true;
    // }
    public virtual bool CanBeTarget(XTarget target, params XCamp[] able_camps) {
        if (!in_board) return false;
        if (target == XTarget.ANY || target == XTarget.GRID) {
            if (able_camps.Contains(camp)) return true;
        }
        if (state == GridState.HAVING && bind_chess.CanBeTarget(target, able_camps)) return true;
        return false;
    }

    public override void BeAttack(int atk, XActor source_actor, AttackType atk_type = AttackType.NORMAL, UnityAction after_callback = null) {
        base.BeAttack(atk, source_actor, atk_type, after_callback);
    }
    // public void BeAttack(int atk, UnityAction after_callback = null) {
    //     foreach (var shield_buff in GetBuffs<IShieldBuff>()) {
    //         atk = shield_buff.BeAttack(atk, this);
    //     }
    //     hp = Mathf.Max(hp - atk, 0);
    //     var hp_effect = FM.LoadHpDecreaseEffect(this, atk);

    //     if (hp == 0) {
    //         BeInjured();
    //     }
    //     if (after_callback != null) {
    //         after_callback();
    //     }

    //     // Sequence seq1 = DOTween.Sequence();
    //     // seq1.AppendInterval(0.4f);
    //     // seq1.AppendCallback(() => {
    //     //     if (hp == 0) {
    //     //         BeInjured();
    //     //     }
    //     //     if (after_callback != null) {
    //     //         after_callback();
    //     //     }
    //     // });
    // }

    protected override void BeInjured(XActor source_actor) {
        base.BeInjured(source_actor);
        GameInfo.RemoveActor(this);
        XGrid xgrid = FM.LoadGrid(new(server_id, grid_position, GridType.AREA, level, XCamp.NEUTRAL));
        if (state == GridState.HAVING) {
            var xchess = bind_chess;
            xchess.ChangeGrid(xgrid);
        }
        DestroyGameObject();
    }




    // private Vector3 offset;
    // private Vector3 last_mouse_point;
    // private void OnMouseDown() {
    //     if (EventSystem.current.IsPointerOverGameObject())
    //         return;
    //     last_mouse_point = Input.mousePosition;
    //     // offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    // }
    // private void OnMouseUp() {
    //     if (EventSystem.current.IsPointerOverGameObject())
    //         return;
    //     if (last_mouse_point != null && Vector3.Distance(Input.mousePosition, last_mouse_point) <= 0.1f) {
    //         if (GM.interact_queue.selected_skill != null) {
    //             GM.interact_queue.selected_skill.OnSelectGrid(this);
    //         }
    //     }
    // }
    // void OnMouseUpAsButton() //鼠标按下
    // {
    //     // offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    // }
    // void OnMouseDrag()//鼠标持续按下
    // {
    //     Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
    //     transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    // }
}