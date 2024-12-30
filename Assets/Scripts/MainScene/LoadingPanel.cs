using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
public class LoadingPanel : MonoBehaviour {
    private static LoadingPanel _instance;
    public static LoadingPanel Ins {
        get {
            return _instance;
        }
    }
    public GameObject loading_panel_go;
    public TMP_Text loading_tip_text;
    public TMP_Text player0_name_text, player1_name_text;

    private CanvasGroup canvas_group;
    public Coroutine coro;

    private void Awake() {
        _instance = this;
        canvas_group = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(gameObject);
    }
    public void Init(List<string> player_name_list) {
        player0_name_text.text = player_name_list[0];
        player1_name_text.text = player_name_list[1];
        loading_panel_go.SetActive(true);
        Debug.Log(gameObject.activeSelf);
        coro = StartCoroutine(LoadingTipIEnumerator());
    }
    public void End() {
        StopCoroutine(coro);
        DOTween.To(() => canvas_group.alpha,
                   (x) => canvas_group.alpha = x,
                   0, 1.5f).OnComplete(() => {
                       loading_panel_go.SetActive(false);
                       Destroy(gameObject);
                   });
    }
    IEnumerator LoadingTipIEnumerator() {
        loading_tip_text.text = "Loading.";
        int times = 1;
        while (true) {
            yield return new WaitForSeconds(0.5f);
            times = times % 3 + 1;
            loading_tip_text.text = "Loading" + new string('.', times);
        }
    }
}