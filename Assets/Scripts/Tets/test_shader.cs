using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ToolI;
using LitJson;
using System.Linq;
using TMPro;

class test_shader : MonoBehaviour {
    public SpriteRenderer spr;
    public float delta;

    public float angle = 0;
    private void Start() {
        StartCoroutine(Ceita());
    }
    IEnumerator Ceita() {
        int times = 359;
        while (times > 0) {
            angle += delta;
            SetSpriteRendererFill(angle);
            yield return new WaitForSeconds(0.1f);
            times--;
        }
    }
    public void SetSpriteRendererFill(float angle) {
        Debug.Log(gameObject.name + angle);
        Material mat = spr.material;

        angle = Mathf.Clamp(angle, 0, 360);
        mat.SetFloat("_Angle", angle);
    }

}