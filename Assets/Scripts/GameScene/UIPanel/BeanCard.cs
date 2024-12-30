using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


public class BeanCard : BaseBehaviour {
    public RectTransform rect_trans { get { return GetComponent<RectTransform>(); } }
    public Image image;
    public Sprite[] sprites;
    public void Init() {
        // image.sprite = sprites[1];
    }
    public void SetHave() {
        image.sprite = sprites[0];
    }
    public void SetNotHave() {
        image.sprite = sprites[1];
    }
}