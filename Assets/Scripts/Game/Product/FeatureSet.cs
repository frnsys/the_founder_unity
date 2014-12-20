/*
 * The set of "features" which affect the performance of a product.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class FeatureSet {
    public static int baseProgress = 1000;

    public int design = 0;
    public int engineering = 0;
    public int marketing = 0;

    // Total feature points.
    public int total {
        get { return design + engineering + marketing; }
    }

    // Progress required for the nth point.
    public static float ProgressRequired(string feature, int n, Product p, Company c) {
        float progress = Fibonacci(n+2) * baseProgress;
        progress *= p.difficulty;

        switch (feature) {
            case "design":
                progress /= c.AggregateWorkerSkill("creativity");
                break;
            case "engineering":
                progress /= c.AggregateWorkerSkill("cleverness");
                break;
            case "charisma":
                progress /= c.AggregateWorkerSkill("marketing");
                break;
            default:
                break;
        }

        return progress;
    }

    public float TotalProgressRequired(Product p, Company c) {
        float progress = 0;
        progress += ProgressRequired("design", design, p, c);
        progress += ProgressRequired("engineering", engineering, p, c);
        progress += ProgressRequired("marketing", marketing, p, c);
        return progress;
    }

    public static int Fibonacci(int n) {
        if (n == 0)
            return 0;
        else if (n == 1)
            return 1;
        else
            return Fibonacci(n-1) + Fibonacci(n-2);
    }
}
