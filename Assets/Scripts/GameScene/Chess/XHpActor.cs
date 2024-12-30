using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Networking.Types;
using UnityEngine.Events;

public class XHpActor : XActor {
    // public int hp;
    // public int max_hp;
    [SerializeField]
    private int _max_hp;
    private int _max_hp_by_level {
        get {
            return (int)(Mathf.Pow(1.5f, level) * _max_hp);
        }
    }
    public int max_hp {
        get { return _max_hp_by_level; }
        set { _max_hp = value; }
    }
    private int _hp;
    public int hp {
        get { return _hp; }
        set {
            _hp = value;
            UpdateHP();
        }
    }
    protected virtual void UpdateHP() { }

    // 攻击棋子
    public virtual void Attack(int atk, XHpActor to_chess, AttackType atk_type = AttackType.NORMAL, UnityAction after_callback = null) {
        if (atk_type.HasFlag(AttackType.IS_SETTLE)) {
            foreach (var attach_attack in GetBuffs<IAttachAttackBuff>()) {
                to_chess.BeAttack(attach_attack.GetDeltaAttack(to_chess), this, atk_type ^ AttackType.IS_SETTLE);
            }
            foreach (var attach_attack in GetBuffs<IAttachRealAttackBuff>()) {
                to_chess.BeAttack(attach_attack.GetDeltaRealAttack(to_chess), this, (atk_type ^ AttackType.IS_SETTLE) | AttackType.IS_IGNORE_SHIELD);
            }
        }
        to_chess.BeAttack(atk, this, atk_type, after_callback);

        if (atk_type.HasFlag(AttackType.IS_SETTLE)) {
            foreach (var after_attack_buff in GetBuffs<IAfterAttackBuff>()) {
                after_attack_buff.AfterAttack(to_chess);
            }
        }
    }
    public virtual void BeAttack(int atk, XActor source_actor, AttackType atk_type = AttackType.NORMAL, UnityAction after_callback = null) {
        if (!atk_type.HasFlag(AttackType.IS_IGNORE_BE_ATK)) {
            var tmp_atk = atk;
            foreach (var be_attack_buff in GetBuffs<IBeAttackBuff>()) {
                tmp_atk += be_attack_buff.GetDeltaBeAttack();
            }
            atk = Mathf.Max(tmp_atk, 0);
        }
        if (!atk_type.HasFlag(AttackType.IS_IGNORE_SHIELD)) {
            foreach (var shield_buff in GetBuffs<IShieldBuff>()) {
                atk = shield_buff.BeAttack(atk, this);
            }
        }

        var hp_effect = FM.LoadHpDecreaseEffect(this, atk);
        var de_hp = Mathf.Min(atk, hp);
        hp = Mathf.Max(hp - atk, 0);

        if (!atk_type.HasFlag(AttackType.IS_IGNORE_COUNTER_ATK)) {
            var counter_atk = 0;
            foreach (var counter_attack_buff in GetBuffs<ICounterAttackBuff>()) {
                counter_atk += counter_attack_buff.GetCounterAttack(de_hp, source_actor, this);
            }
            if (source_actor is XHpActor source_xhp) {
                if (counter_atk > 0)
                    source_xhp.BeAttack(counter_atk, this, atk_type | AttackType.IS_IGNORE_COUNTER_ATK);
            }
        }
        // TODO

        RefreshBuffEffect();

        // if(hp==0)
        if (atk_type.HasFlag(AttackType.IS_SETTLE)) {
            Sequence seq1 = DOTween.Sequence();
            seq1.AppendInterval(0.4f);
            seq1.AppendCallback(() => {
                if (hp == 0) {
                    BeInjured(source_actor);
                }

                if (after_callback != null) {
                    after_callback();
                }
            });
        }
    }
    protected virtual void BeInjured(XActor source_actor) { }

}