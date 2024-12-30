using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;


public class BeanQueue : BaseBehaviour {
    public List<BeanCard> beans;
    public Transform beans_trans;
    public TMP_Text bean_text;
    public void Awake() {
        EM.bean_change.AddListener(BeanChange);
    }
    public void Init() {
        beans = new List<BeanCard>();
        for (int i = 0; i < GameInfo.max_bean; ++i) {
            var bean = FM.LoadBean();
            bean.transform.SetParent(beans_trans);
            bean.rect_trans.localScale = Vector3.one;
            beans.Add(bean);
        }
        BeanChange();
    }
    public void BeanChange() {
        bean_text.text = GameInfo.bean.ToString();
        for (int i = 0; i < beans.Count; ++i) {
            if (i < GameInfo.bean) beans[i].SetHave();
            else beans[i].SetNotHave();
        }

    }
}