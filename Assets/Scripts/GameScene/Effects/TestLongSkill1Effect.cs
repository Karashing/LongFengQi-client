using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.Events;

public class TestLongSkill1Effect : XEffectAction {
    public Sprite default_spr, out_spr, in_spr;
    public SpriteRenderer out_sr, in_sr;
    public Transform out_trans, in_trans;
    public Color target_color;

    // public override void Init(UnityAction xcallback) {
    //     callback = xcallback;
    // }
    public override void Play() {
        out_trans.localScale = new Vector3(7f, 7f, 7f);
        out_sr.sprite = default_spr;
        out_sr.color = new Color(1, 1, 1, 0f);
        Sequence seq0 = DOTween.Sequence();
        seq0.Append(out_sr.DOColor(new Color(1, 1, 1, 0.4f), 1.3f).SetEase(Ease.OutQuad));
        seq0.Join(out_trans.DOScale(1.4f, 1.3f).SetEase(Ease.OutCubic));
        seq0.AppendInterval(0.2f);
        seq0.Append(out_sr.DOColor(target_color, 0.1f));
        seq0.Join(out_trans.DOScale(1f, 0.1f));
        seq0.AppendCallback(() => { out_sr.sprite = out_spr; });
        seq0.AppendInterval(1);
        seq0.Append(out_sr.DOColor(new Color(1, 1, 1, 0), 0.5f));
        seq0.Join(out_trans.DOScale(0f, 0.5f).SetEase(Ease.InCubic));


        in_trans.localScale = new Vector3(0, 0, 0);
        in_sr.sprite = default_spr;
        in_sr.color = new Color(1, 1, 1, 0.6f);
        Sequence seq1 = DOTween.Sequence();
        seq1.Append(in_sr.DOColor(target_color, 1.3f));
        seq1.Join(in_trans.DOScale(0.7f, 1.3f).SetEase(Ease.OutCubic));
        seq1.AppendInterval(0.2f);
        seq1.Append(in_sr.DOColor(target_color, 0.1f));
        seq1.Join(in_trans.DOScale(1f, 0.1f));
        seq0.AppendCallback(() => { in_sr.sprite = in_spr; });
        seq1.AppendCallback(() => { callback(); });
        seq1.AppendInterval(1);
        seq1.Append(in_sr.DOColor(new Color(1, 1, 1, 0), 0.5f));
        seq1.Join(in_trans.DOScale(0f, 0.5f).SetEase(Ease.InCubic));
        seq1.OnComplete(() => {
            Destroy(gameObject);
        });
    }

}
