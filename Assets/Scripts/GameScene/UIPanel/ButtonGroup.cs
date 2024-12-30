using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

[Serializable]
public class ButtonUI : BaseGame {
    public Button button;
    public Image image;
    public TMP_Text text;
}

public class ButtonGroup : BaseBehaviour {
    public List<ButtonUI> buttons;
    public void OnSelect(int button_id) {
        if (button_id >= 0 && button_id < buttons.Count) {
            OnSelect(buttons[button_id]);
        }
    }
    private void OnSelect(ButtonUI button) {
        foreach (var xbutton in buttons) {
            xbutton.image.color = new Color(0, 0, 0, 0.5f);
            xbutton.text.color = new Color(1, 1, 1);
        }
        button.image.color = new Color(1, 0.99f, 0.87f, 0.9f);
        button.text.color = new Color(0, 0, 0);
    }
    // private void Awake() {
    //     foreach (var button in buttons) {
    //         button.button.onClick.AddListener(() => OnSelect(button));
    //     }
    // }
}