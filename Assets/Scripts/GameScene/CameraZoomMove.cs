using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ToolI;
using DG.Tweening;

public class CameraZoomMove : BaseBehaviour {
    private Camera mCamera;     // 参与变换的摄像机
    public Vector2 rangeSize = new Vector2(3, 5);     // 通过这个来控制Size缩放量
    // public Vector2 mapRange = new Vector2(13, 8);   // 通过这个来控制可视范围
    public RectV2 rangeRect = new RectV2(-13, -8, 13, 8);   // 通过这个来控制可视范围
    public float moveRate = 1f;
    public float dragDecelerationRate = 0.1f; //减速率

    private RectV2 rect, limRect;// 限制运动范围矩形
    private Vector3 lastMousePoint;
    private bool can_move = true;
    private bool isMouseDown;
    public int ban_move = 0;

    private void Awake() {
        mCamera = GetComponent<Camera>();
    }

    private void Start() {
        InitCamera();
    }

    private void Update() {
        AnchorZoom();
        if (can_move && ban_move == 0)
            ScreenMove();
    }

    public void InitCamera() {
        limRect = new RectV2(rangeRect.lx - 2, rangeRect.ly - 1, rangeRect.rx + 2, rangeRect.ry + 1);
        Debug.Log(rangeRect.lx + " " + rangeRect.ly + " " + rangeRect.rx + " " + rangeRect.ry);
    }

    public void FocusOnPosition(Vector3 world_point, float duration = 0.5f) {
        var target_position = new Vector3(world_point.x, world_point.y, mCamera.transform.position.z);
        can_move = false;
        isMouseDown = false;
        mCamera.transform.DOMove(target_position, duration).OnComplete(
            () => {
                UpdateNowCameraRect();
                can_move = true;
            }
        );
    }

    /// <summary>
    /// 根据视点缩放
    /// </summary>
    private void AnchorZoom() {
        float v = Input.GetAxis("Mouse ScrollWheel") * 3;
        if (v == 0) return;
        float old_size = mCamera.orthographicSize; // 计算摄像机原来的大小
        float size = Mathf.Clamp(old_size - v, rangeSize.x, rangeSize.y); // 计算移动后的大小
        float pro = (old_size - size) / old_size; // 计算摄像机的缩放倍数 
        // 公式
        Vector3 position = mCamera.transform.position + (mCamera.ScreenToWorldPoint(Input.mousePosition) - mCamera.transform.position) * pro;

        mCamera.orthographicSize = size; // 应用大小缩放
        mCamera.transform.position = new Vector3(position.x, position.y, mCamera.transform.position.z); //位置缩放
        UpdateNowCameraRect();
    }
    private bool IsMoving() {
        if (Input.GetMouseButton(1)) return true;
        if (Input.GetMouseButtonDown(0)) {
            if (EventSystem.current.IsPointerOverGameObject() == false) return true;
        }
        if (Input.GetMouseButton(0)) {
            if (isMouseDown) return true;
        }
        return false;
    }
    private void ScreenMove() {
        if (IsMoving()) {
            if (isMouseDown) {
                Vector3 move = Camera.main.ScreenToWorldPoint(Input.mousePosition) - lastMousePoint;
                // Debug.Log(move);
                move = (move.x * -mCamera.transform.right) + (move.y * -mCamera.transform.up);
                mCamera.transform.position += move;
                UpdateNowCameraRect();
            }
            lastMousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isMouseDown = true;

            Vector3 offset = rect.Offset(rangeRect);
            Vector3 postion = mCamera.transform.position;
            if (offset.x != 0)
                postion.x -= RubberDelta(offset.x, rangeRect.width);
            if (offset.y != 0)
                postion.y -= RubberDelta(offset.y, rangeRect.height);

            mCamera.transform.position = postion;
            UpdateNowCameraRect();
            mCamera.transform.position -= (Vector3)rect.Offset(limRect);

        }
        else if (rect.isOffset(rangeRect)) { //限制摄像机运动范围
            isMouseDown = false;
            if (rect.ly < rangeRect.ly) {
                float d = rangeRect.ly - rect.ly;
                mCamera.transform.position += new Vector3(0, d * dragDecelerationRate, 0);
            }
            if (rect.ry > rangeRect.ry) {
                float d = rangeRect.ry - rect.ry;
                mCamera.transform.position += new Vector3(0, d * dragDecelerationRate, 0);
            }
            if (rect.lx < rangeRect.lx) {
                float d = rangeRect.lx - rect.lx;
                mCamera.transform.position += new Vector3(d * dragDecelerationRate, 0, 0);
            }
            if (rect.rx > rangeRect.rx) {
                float d = rangeRect.rx - rect.rx;
                mCamera.transform.position += new Vector3(d * dragDecelerationRate, 0, 0);
            }
            UpdateNowCameraRect();
        }
        else {
            isMouseDown = false;
        }
    }
    private float RubberDelta(float overStretching, float viewSize) {
        return (1 - (1 / ((Mathf.Abs(overStretching) * 0.95f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
    }
    private void UpdateNowCameraRect() {
        var postion = Camera.main.ScreenToWorldPoint(Vector3.zero);
        var size = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        rect = new RectV2(postion.x, postion.y, size.x, size.y);
    }
}
