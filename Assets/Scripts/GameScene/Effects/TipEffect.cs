using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;

public class TipEffect : BaseBehaviour {
    public Transform sprite_trans;
    public SpriteRenderer sprite_renderer;
    public Vector3Int pos;
    private List<Tween> tweens;
    public void Init(Vector3Int xpos) {
        pos = xpos;
        if (GameInfo.grid_dict.ContainsKey(pos)) {
            var xgrid = GameInfo.grid_dict[pos];
            if (xgrid.state == GridState.HAVING) {
                sprite_renderer.color = FM.GetCampColor(xgrid.bind_chess.camp);
            }
            else if (xgrid.camp == XCamp.ENEMY || xgrid.camp == XCamp.PUBLIC_ENEMY) {
                sprite_renderer.color = FM.GetCampColor(xgrid.camp);
            }
            else {
                sprite_renderer.color = Color.white;
            }
        }
        else {
            sprite_renderer.color = Color.white;
        }
        transform.position = GM.grid_map.GetCellCenterWorld(pos);
        Tween tween = sprite_trans.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutQuad);
        tweens = new List<Tween> {
            tween
        };
    }
    public void End() {
        foreach (var tween in tweens) {
            tween.Kill();
        }
        gameObject.SetActive(false);
        Destroy(gameObject);
    }




    // private Vector3 offset;
    private Vector3 last_mouse_point;
    private void OnMouseDown() {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        last_mouse_point = Input.mousePosition;
        // offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnMouseUp() {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (last_mouse_point != null && Vector3.Distance(Input.mousePosition, last_mouse_point) <= 0.1f) {
            if (GM.interact_queue.selected_skill != null) {
                Debug.Log(pos);
                GM.interact_queue.selected_skill.OnSelectPosition(pos);
            }
        }
        Debug.Log($"mouse: {Input.GetMouseButton(0)}");
    }
    private void OnMouseEnter() {
        transform.localScale = new(1.1f, 1.1f, 1.1f);
    }
    private void OnMouseExit() {
        transform.localScale = new(1f, 1f, 1f);
    }
    void OnMouseUpAsButton() //鼠标按下
    {
        // offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    // void OnMouseDrag()//鼠标持续按下
    // {
    //     Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
    //     transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    // }
}
