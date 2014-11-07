/*
 * Personality profiles for AI.
 */

using UnityEngine;
using System.Collections;

[System.Serializable]
public class Personality {

    // Influences how hostile they are against others.
    // 0 = friendly, unlikely to do anything hostile.
    // 1 = will try to sue the shit out of everyone.
    public float aggression = 0;

    // Influences how willing they are to cooperate.
    // 0 = unwilling to cooperate on anything.
    // 1 = hellooo wage fixing!
    public float cooperativeness = 0;

    // Influences how lucky they are.
    // <1 = a penalty to luck
    // >1 = a bonus to luck
    public float luck = 0;
}
