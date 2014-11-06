/*
 * Research is accumulated to achieve a Discovery.
 */

using UnityEngine;
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
        technical = t;
        design = d;
    }

    public static Research operator +(Research left, Research right) {
        return new Research(
                    left.management + right.management,
                    left.technical + right.technical,
                    left.design + right.design);
    }

    public static float operator /(Research left, Research right) {
        // The left values can't be greater than the right's values.
        float m = left.management > right.management ? right.management : left.management;
        float t = left.technical > right.technical ? right.technical : left.technical;
        float d = left.design > right.design ? right.design : left.design;

        return (m + t + d)/right.total;
    }

    public static bool operator >=(Research left, Research right) {
        return (left/right >= 1f);
    }

    public static bool operator <=(Research left, Research right) {
        return (left/right <= 1f);
    }
}


