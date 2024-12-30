using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;



// BuffAnimID: 540 650 725 774 782 783 804

public interface IShieldBuff {
    public abstract int BeAttack(int atk, XActor xactor = null);
    public abstract int GetShield(XActor xactor = null);
}
public interface ISpeedBuff { // 速度Buff
    public float GetDeltaSpeed();
}
public interface IMultiSpeedBuff { // 速度Buff
    public float GetMultiSpeed();
}
public interface IAttackBuff { // 攻击力Buff
    public int GetDeltaAttack();
}
public interface IActionBuff { // 行动Buff
    public bool CanAction();
}
public interface IBeAttackBuff { // 受击Buff
    public int GetDeltaBeAttack();
}
public interface ICounterAttackBuff { // 反击Buff
    public int GetCounterAttack(int atk, XActor from_actor = null, XActor to_actor = null);
}
public interface IAttachAttackBuff { // 附带伤害Buff
    public int GetDeltaAttack(XHpActor target);
}
public interface IAttachRealAttackBuff { // 附带真伤Buff
    public int GetDeltaRealAttack(XHpActor target);
}
public interface IAfterAttackBuff { // 攻击后Buff
    public void AfterAttack(XHpActor target);
}
public interface ICoinBeanBuff { // 额外金币收货时Buff
    public int GetDeltaCoin();
}
public interface ITimesBuff { // 计数Buff
    public int GetTimes();
    public void AddTimes(int delta_times);
}




[Serializable]
[Flags]
public enum BuffTriggerType {
    NONE = 0,
    ENTER_ACTION = 1,
    QUIT_ACTION = 1 << 1,
    BEFORE_ACT = 1 << 2,
    AFTER_ACT = 1 << 3,
    BEFORE_REACT = 1 << 4,
    AFTER_REACT = 1 << 5,
}

[Serializable]
public enum BuffLifetimeType {
    PERMANENT = 0,
    TIMES_LIMIT = 1,
    CONDITION = 1,
}

[Serializable]
public class XBuffLifetime {
    public BuffLifetimeType type;
    public virtual bool IsEndAfterTrigger() {
        return true;
    }
}

[Serializable]
public class BuffLifetimeTimesLimit : XBuffLifetime {
    public int times_limit;
    public BuffLifetimeTimesLimit(int xtimes_limit) {
        type = BuffLifetimeType.TIMES_LIMIT;
        times_limit = xtimes_limit;
    }
    public override bool IsEndAfterTrigger() {
        times_limit--;
        return times_limit <= 0;
    }
}

[Serializable]
public class BuffLifetimeConditionLimit : XBuffLifetime {
    public Func<bool> condition_callback;
    public BuffLifetimeConditionLimit(Func<bool> lifetime_end_condition) {
        type = BuffLifetimeType.CONDITION;
        condition_callback = lifetime_end_condition;
    }
    public override bool IsEndAfterTrigger() {
        return condition_callback();
    }
}

[Serializable]
public class BuffLifetimePermanent : XBuffLifetime {
    public BuffLifetimePermanent() {
        type = BuffLifetimeType.PERMANENT;
    }
    public override bool IsEndAfterTrigger() {
        return false;
    }
}