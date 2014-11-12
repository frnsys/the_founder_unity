/*
 * Research is accumulated to achieve a Discovery.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class Research {
    // Different kinds of research points.
    public float management = 0;
    public float technical = 0;
    public float design = 0;

    public float total {
        get { return management + technical + design; }
    }

    public Research(float m, float t, float d) {
        management = m;
        technical  = t;
        design     = d;
    }

    public static Research operator +(Research left, Research right) {
        return new Research(
                    left.management + right.management,
                    left.technical  + right.technical,
                    left.design     + right.design);
    }

    public static float operator /(Research left, Research right) {
        // The left values can't be greater than the right's values.
        float m = Math.Min(left.management, right.management);
        float t = Math.Min(left.technical,  right.technical);
        float d = Math.Min(left.design,     right.design);

        return (m + t + d)/right.total;
    }

    public static bool operator >=(Research left, Research right) {
        return left/right >= 1f;
    }

    public static bool operator <=(Research left, Research right) {
        return left/right <= 1f;
    }

    public static bool operator ==(Research left, Research right) {
        if (left.management - right.management != 0)
            return false;

        if (left.technical - right.technical != 0)
            return false;

        if (left.design - right.design != 0)
            return false;

        return true;
    }

    public static bool operator !=(Research left, Research right) {
        return !(left == right);
    }
}


