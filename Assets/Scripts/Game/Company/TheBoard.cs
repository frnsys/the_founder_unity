/*
 * The board can make you resign SO KEEP THEM HAPPY
 */

using UnityEngine;

[System.Serializable]
public class TheBoard {
    public float happiness = 10;
    public float profitTarget = 20000;
    public float lastProfit = 20000;
    public float lastProfitTarget = 0;
    public float desiredGrowth = 0.12f;

    public enum Status {
        ECSTATIC,
        PLEASED,
        CONTENT,
        UNHAPPY,
        RAGING
    }

    public Status status {
        get {
            if (happiness >= 50) {
                return Status.ECSTATIC;
            } else if (happiness >= 35) {
                return Status.PLEASED;
            } else if (happiness >= 10) {
                return Status.CONTENT;
            } else if (happiness > 0) {
                return Status.UNHAPPY;
            } else {
                return Status.RAGING;
            }
        }
    }

    public float EvaluatePerformance(float profit) {
        float growth = profit/lastProfit - 1;

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
        lastProfitTarget = profitTarget;
        profitTarget *= 1 + desiredGrowth;
        lastProfit = profit;

        return growth;
    }

    // TO DO these should become emoji!
    public string BoardStatus() {
        switch (status) {
            case Status.ECSTATIC:
                return "The Board is ecstatic!";
            case Status.PLEASED:
                return "The Board is pleased.";
            case Status.CONTENT:
                return "The Board is content.";
            case Status.UNHAPPY:
                return "Take care. The board is NOT happy.";
            case Status.RAGING:
                return "Watch your back - the board is furious!";
            default:
                return "The Board is indifferent.";
        }
    }
}
