using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class FeatureSet : Dictionary<string, int> {
    public FeatureSet() {
        string[] features = new string[] {
            "Delight",
            "Usability",
            "Security",
            "Stability",
            "Branding",
            "Marketing"
        };
        foreach (string key in features) {
            base.Add(key, 0);
        }
    }

    public FeaturePoints Increment(string feature, FeaturePoints points) {
        switch (feature) {
            case "Delight":
                // CRE+CRE
                if (points.creativity >= 2) {
                    points.creativity -= 2;
                    this["Delight"]++;
                }
                break;
            case "Usability":
                // CRE+CLE
                if (points.creativity >= 1 && points.cleverness >= 1) {
                    points.creativity--;
                    points.cleverness--;
                    this["Usability"]++;
                }
                break;
            case "Security":
                // CLE+CHA
                if (points.cleverness >= 1 && points.charisma >= 1) {
                    points.cleverness--;
                    points.charisma--;
                    this["Security"]++;
                }
                break;
            case "Stability":
                // CLE+CLE
                if (points.cleverness >= 2) {
                    points.cleverness -= 2;
                    this["Stability"]++;
                }
                break;
            case "Branding":
                // CRE+CHA
                if (points.creativity >= 1 && points.charisma >= 1) {
                    points.creativity--;
                    points.charisma--;
                    this["Branding"]++;
                }
                break;
            case "Marketing":
                // CHA+CHA
                if (points.charisma >= 2) {
                    points.charisma -= 2;
                    this["Marketing"]++;
                }
                break;
        }
        return points;
    }


    public FeaturePoints Decrement(string feature, FeaturePoints points) {
        switch (feature) {
            case "Delight":
                // CRE+CRE
                if (this["Delight"] > 0) {
                    points.creativity += 2;
                }
                this["Delight"]--;
                break;
            case "Usability":
                // CRE+CLE
                if (this["Usability"] > 0) {
                    points.creativity++;
                    points.cleverness++;
                }
                this["Usability"]--;
                break;
            case "Security":
                // CLE+CHA
                if (this["Security"] > 0) {
                    points.cleverness++;
                    points.charisma++;
                }
                this["Security"]--;
                break;
            case "Stability":
                // CLE+CLE
                if (this["Stability"] > 0) {
                    points.cleverness += 2;
                }
                this["Stability"]--;
                break;
            case "Branding":
                // CRE+CHA
                if (this["Branding"] > 0) {
                    points.creativity++;
                    points.charisma++;
                }
                this["Branding"]--;
                break;
            case "Marketing":
                // CHA+CHA
                if (this["Marketing"] > 0) {
                    points.charisma += 2;
                }
                this["Marketing"]--;
                break;
        }
        return points;
    }
}
