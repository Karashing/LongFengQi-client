using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using ToolI;

[Serializable]
class A {
    public int x;
}
[Serializable]
class B : A {
    public string y;
}

// public class test : BaseBehaviour {
//     public SpriteRenderer spriteRenderer;
//     private void Awake() {
//         // spriteRenderer.sprite = FM.GetShopButtonSprite(true);
//         B b = new B() {
//             x = 123,
//             y = "321",
//         };
//         A a = (A)b;
//         string test = JsonUtility.ToJson(a);
//         Debug.Log(test);
//         A test_a = JsonUtility.FromJson<A>(test);
//         Debug.Log(test_a);
//         A test_b = JsonUtility.FromJson<B>(test);
//         Debug.Log(test_b);
//         Debug.Log((test_b).x);
//         Debug.Log(((B)test_b).y);
//     }
// }

public class test : MonoBehaviour {
    public SpriteRenderer spr;

    private void Start() {
        Debug.Log(TileMap6.GetDistance(new(0, -5), new(2, -2)));
    }

    private void OnMouseDown() {
        Debug.Log("mousedown");
    }
}