using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.Events;
using TMPro;

public class test_canvas : MonoBehaviour {
    public RectTransform canvas_trans;
    public Canvas canvas;
    public TMP_Text debug_text;
    private void Update() {
        debug_text.text = Input.mousePosition.ToString();
    }


}
