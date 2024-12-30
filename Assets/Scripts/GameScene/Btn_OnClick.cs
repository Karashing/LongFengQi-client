
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Btn_OnClick : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
    // 按压的持续时间
    public float pressDurationTime = 1;
    // 按压的响应次数
    public bool responseOnceByPress = false;
    // 双击的间隔时间
    public float doubleClickIntervalTime = 0.2f;
    // 拖动的间隔时间
    public float dragIntervalTime = 0.2f;
    // 拖动的鼠标间隔距离
    public float dragIntervalPos = 0.01f;

    public UnityEvent onDoubleClick;
    public UnityEvent onSingleClick;
    public UnityEvent onPress;
    public UnityEvent onClick;
    public UnityEvent onDrag;

    private bool isDown = false;
    private bool isPress = false;
    private bool isDrag = false;
    private float downTime = 0;

    private float clickIntervalTime = 0;
    private int clickTimes = 0;

    private Vector3 mousePosLast = Vector3.zero;//点击后的拖动位置

    Btn_OnClick btn;

    void Start() {
        btn = GetComponent<Btn_OnClick>();
        btn.onClick.AddListener(Click);
        btn.onPress.AddListener(Press);
        btn.onDoubleClick.AddListener(DoubleClick);
        btn.onSingleClick.AddListener(SingleClick);
        btn.onDrag.AddListener(Drag);
    }

    void Click() {
        Debug.Log("点击");
    }

    void Press() {
        Debug.Log("按压");
    }

    void DoubleClick() {
        Debug.Log("双击");
    }

    void SingleClick() {
        Debug.Log("单击");
    }

    void Drag() {
        Debug.Log("拖动");
    }

    void Update() {

        if (isDown) {
            if (responseOnceByPress && isPress) {
                return;
            }
            downTime += Time.deltaTime;
            isDrag = Vector3.Distance(Input.mousePosition, mousePosLast) > dragIntervalPos;
            if (downTime > pressDurationTime && !isDrag) {
                isPress = true;
                onPress.Invoke();
            }
            if (downTime > dragIntervalTime && isDrag) {
                onDrag.Invoke();
            }
        }
        if (clickTimes >= 1) {
            clickIntervalTime += Time.deltaTime;
            if (clickIntervalTime >= doubleClickIntervalTime) {
                if (clickTimes >= 2) {
                    onDoubleClick.Invoke();
                }
                else {
                    onSingleClick.Invoke();
                }
                clickTimes = 0;
                clickIntervalTime = 0;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (!isPress) {
            clickTimes += 1;
            onClick.Invoke();
        }
        else
            isPress = false;
    }

    public void OnPointerDown(PointerEventData eventData) {
        isDown = true;
        downTime = 0;
        mousePosLast = Input.mousePosition;
    }

    public void OnPointerExit(PointerEventData eventData) {
        isDown = false;
        isPress = false;
    }

    public void OnPointerUp(PointerEventData eventData) {
        isDown = false;
    }
}
