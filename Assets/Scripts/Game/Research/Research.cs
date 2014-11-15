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
    public float charisma   = 0;
    public float cleverness = 0;
    public float creativity = 0;

    public float total {
        get { return charisma + cleverness + creativity; }
    }

    public Research(float ch, float cl, float cr) {
        charisma   = ch;
        cleverness = cl;
        creativity = cr;
    }
    public Research() {
        charisma   = 0;
        cleverness = 0;
        creativity = 0;
    }

    public static Research operator +(Research left, Research right) {
        return new Research(
                    left.charisma   + right.charisma,
                    left.cleverness + right.cleverness,
                    left.creativity + right.creativity);
    }

    public static float operator /(Research left, Research right) {
        // The left values can't be greater than the right's values.
        float m = Math.Min(left.charisma,   right.charisma);
        float t = Math.Min(left.cleverness, right.cleverness);
        float d = Math.Min(left.creativity, right.creativity);

        return (m + t + d)/right.total;
    }

    public static bool operator >=(Research left, Research right) {
        return left/right >= 1f;
    }

    public static bool operator <=(Research left, Research right) {
        return left/right <= 1f;
    }

    public static bool operator ==(Research left, Research right) {
        return left.charisma   - right.charisma   == 0 &&
               left.cleverness - right.cleverness == 0 &&
               left.creativity - right.creativity == 0 ;
    }

    public static bool operator !=(Research left, Research right) {
        return !(left == right);
    }
}


