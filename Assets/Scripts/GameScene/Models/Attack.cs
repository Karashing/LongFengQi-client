using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEditor;
using JetBrains.Annotations;
using System.ComponentModel;


[Serializable]
[Flags]
public enum AttackType {
    NONE = 0,
    IS_SETTLE = 1, // 结算伤害
    IS_IGNORE_SHIELD = 1 << 1, // 忽略护盾
    IS_IGNORE_BE_ATK = 1 << 2, // 忽略易伤
    IS_IGNORE_COUNTER_ATK = 1 << 3, // 忽略反击
    IS_IGNORE_ATTACH_ATK = 1 << 4, // 忽略附加伤害

    IGNORE_ALL_BUFF = IS_IGNORE_SHIELD | IS_IGNORE_BE_ATK | IS_IGNORE_COUNTER_ATK | IS_IGNORE_ATTACH_ATK, // 忽略所有Buff
    NORMAL = IS_SETTLE,
}