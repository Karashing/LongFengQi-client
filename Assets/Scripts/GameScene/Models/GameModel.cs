
using System;
using UnityEngine;

[Serializable]
public class HP {
    public int max_hp;
    public int hp;
}

[Serializable]
public class Shield {
    public int max_shield;
    public int shield;
    public Shield(int xshield) {
        shield = xshield;
        max_shield = xshield;
    }
    public Shield(int xmax_shield, int xshield) {
        shield = xshield;
        max_shield = xmax_shield;
    }
    public int BeAttack(int atk) {
        if (atk > shield) {
            var remain_atk = atk - shield;
            shield = 0;
            return remain_atk;
        }
        else {
            shield -= atk;
            return 0;
        }
    }
    public void MaxShieldChange(int delta_shield, int lim_shield = -1) {
        if (lim_shield >= 0) {
            delta_shield = Mathf.Min(max_shield + delta_shield, lim_shield) - max_shield;
        }
        max_shield += delta_shield;
        shield += delta_shield;
    }
}