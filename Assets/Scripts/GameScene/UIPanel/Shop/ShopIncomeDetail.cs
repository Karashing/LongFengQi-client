using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ToolI;
using System.Linq;

public class ShopIncomeDetail : BaseBehaviour {
    public RectTransform rect_trans { get { return GetComponent<RectTransform>(); } }
    public TMP_Text coin, describe;
    public void Init(int xcoin, string xdescribe) {
        coin.text = xcoin.ToString();
        describe.text = xdescribe;
    }

}