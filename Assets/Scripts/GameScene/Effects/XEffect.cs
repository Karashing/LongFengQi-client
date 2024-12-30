using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.Events;

public class XEffectAction : BaseBehaviour {
    public UnityAction callback;
    public UnityAction<int> callback1;
    public Vector3 offset_pos;
    public virtual void Init(UnityAction call_back) {
        callback = call_back;
    }
    public virtual void Init(UnityAction<int> call_back) {
        callback1 = call_back;
    }
    public virtual void SetParent(Transform trans) {
        transform.SetParent(trans);
        transform.localPosition = offset_pos;
    }
    public virtual void Play() { }
    public virtual void Kill() {
        Destroy(gameObject);
    }
}