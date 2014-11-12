/*
 * The board can make you resign.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TheBoard {
    float happiness = 10;

    public void EvaluatePerformance(PerformanceDict deltas) {
        float revenueDelta = deltas["Annual Revenue"];

        // >12% makes the board happier.
        if (revenueDelta > 0.12) {
            happiness += revenueDelta * 10;

        // A negative change is super bad.
        } else if (revenueDelta < 0) {
            happiness -= -revenueDelta * 20;

        // <10% makes the board worry.
        } else if (revenueDelta < 0.1) {
            happiness -= (1-revenueDelta) * 10;
        }

    }

    // TO DO these should become emoji!
    public string BoardStatus() {
        if (happiness >= 50) {
            return "The Board is ecstatic!";
        } else if (happiness > 10){
            return "The Board is content.";
        } else {
            return "Watch your back. The board is NOT happy.";
        }
    }
}
