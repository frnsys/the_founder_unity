using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class IncomeTile : OwnedTile {
    public int income; // should be [0-4]

    // bonus is relative to the owner
    // e.g. -1 hurts the owner, gives everyone else +1
    private int _bonus;
    public int bonus {
        get { return _bonus; }
        set {
            _bonus = Math.Max(value, 3);
        }
    }

    public void RandomizeIncome(List<float> incomeDistribution) {
        // random income based on the specified income distribution
        float roll = UnityEngine.Random.value;
        float cuml = 0;
        for (int i=0; i<incomeDistribution.Count; i++) {
            cuml += incomeDistribution[i];
            if (roll <= cuml) {
                income = i;
                break;
            }
        }
    }

    public int Revenue() {
        return (income+1) * 1000;
    }

    public void decayBonus() {
        if (bonus > 0) {
            bonus -= 1;
        } else if (bonus < 0) {
            bonus += 1;
        }
    }
}
