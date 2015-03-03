/*
 * The board can make you resign SO KEEP THEM HAPPY
 */

using UnityEngine;

[System.Serializable]
public class TheBoard {
    public float happiness = 10;
    public float profitTarget = 20000;
    public float lastQuarterProfit = 20000;
    public float desiredGrowth = 0.12f;

    public float EvaluatePerformance(float profit) {
        float growth = profit/lastQuarterProfit - 1;

        // If the target is exceeded, the board is really happy.
        if (growth >= desiredGrowth * 2)
            happiness += growth * 12;

        // If the target is met, the board is happy.
        if (growth >= desiredGrowth) {
            happiness += growth * 10;

        // A negative change is super bad.
        } else if (growth < 0) {
            happiness -= -growth * 20;

        // Otherwise, the growth just becomes a bit more unhappy.
        } else {
            happiness -= (1-growth) * 10;
        }

        // Set the new target.
        profitTarget *= 1 + desiredGrowth;
        lastQuarterProfit = profit;

        return growth;
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
