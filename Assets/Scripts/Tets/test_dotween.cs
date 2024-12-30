using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.Events;

public class test_dotween : MonoBehaviour {
    public SpriteRenderer out_spr, in_spr;
    public Transform out_trans, in_trans;
    public Color target_color;
    public UnityAction callback;

    public void Init(UnityAction xcallback) {
        callback = xcallback;
    }
    async void Start() {
        await Task.Delay(3000);

        out_trans.localScale = new Vector3(5f, 5f, 5f);
        out_spr.color = new Color(1, 1, 1, 0f);
        Sequence seq0 = DOTween.Sequence();
        seq0.Append(out_spr.DOColor(new Color(1, 1, 1, 0.4f), 1.3f));
        seq0.Join(out_trans.DOScale(1.4f, 1.3f).SetEase(Ease.OutQuad));
        seq0.AppendInterval(0.2f);
        seq0.Append(out_spr.DOColor(target_color, 0.1f));
        seq0.Join(out_trans.DOScale(1f, 0.1f));
        seq0.AppendInterval(1);
        seq0.Append(out_spr.DOColor(new Color(1, 1, 1, 0), 0.5f));
        seq0.Join(out_trans.DOScale(0f, 0.5f).SetEase(Ease.InCubic));


        in_trans.localScale = new Vector3(0, 0, 0);
        in_spr.color = new Color(1, 1, 1, 0.6f);
        Sequence seq1 = DOTween.Sequence();
        seq1.Append(in_spr.DOColor(target_color, 1.3f));
        seq1.Join(in_trans.DOScale(0.7f, 1.3f).SetEase(Ease.OutQuad));
        seq1.AppendInterval(0.2f);
        seq1.Append(in_spr.DOColor(target_color, 0.1f));
        seq1.Join(in_trans.DOScale(1f, 0.1f));
        seq1.AppendCallback(() => { callback(); });
        seq1.AppendInterval(1);
        seq1.Append(in_spr.DOColor(new Color(1, 1, 1, 0), 0.5f));
        seq1.Join(in_trans.DOScale(0f, 0.5f).SetEase(Ease.InCubic));
        seq1.OnComplete(() => {
            Destroy(gameObject);
        });
    }

}
