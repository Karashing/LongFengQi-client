using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StarBG : MonoBehaviour {
    private static StarBG _instance;
    public static StarBG Ins {
        get {
            return _instance;
        }
    }
    public Transform camera_trans;
    public Transform start_ring1_trans;
    public Transform start_ring2_trans;
    public ParticleSystem ring0_particle, ring1_particle, ring2_particle;
    private void Awake() {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start() {
        ring0_particle.Simulate(0.0f);
        ring1_particle.Simulate(0.0f);
        ring2_particle.Simulate(0.0f);
        var tween0_0 = start_ring1_trans.DOLocalRotate(new Vector3(2, -1, 0), 60f, RotateMode.WorldAxisAdd)
                                                .SetEase(Ease.InOutQuad);
        var tween0_1 = start_ring1_trans.DOScale(1.1f, 20f)
                                        .SetEase(Ease.InOutQuad);
        tween0_0.SetLoops(-1, LoopType.Yoyo);
        tween0_1.SetLoops(-1, LoopType.Yoyo);

        var tween1_0 = start_ring2_trans.DOLocalRotate(new Vector3(4, -6, 0), 60f, RotateMode.WorldAxisAdd)
                                                .SetEase(Ease.InOutCubic);
        var tween1_1 = start_ring2_trans.DOScale(0.9f, 20f)
                                        .SetEase(Ease.InOutQuad);
        tween1_0.SetLoops(-1, LoopType.Yoyo);
        tween1_1.SetLoops(-1, LoopType.Yoyo);
    }
    public void Play() {
        gameObject.SetActive(true);
        camera_trans.DOLocalRotate(Vector3.zero, 8f).SetEase(Ease.InOutQuad);
        ring0_particle.Play();
        ring1_particle.Play();
        ring2_particle.Play();
    }
    public void Pause() {
        ring0_particle.Simulate(0.0f);
        ring1_particle.Simulate(0.0f);
        ring2_particle.Simulate(0.0f);
        gameObject.SetActive(false);
    }
}
