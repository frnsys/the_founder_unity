/*
 * Manages markets.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MarketManager {
    public enum Market {
        NorthAmerica,
        SouthAmerica,
        Asia,
        Europe,
        Africa,
        Australia,
        Antarctica,
        ExtraTerra,
        Alien
    }

    public static float SizeForMarket(Market m) {
        switch (m) {
            case Market.NorthAmerica:
                return 1.0f;
            case Market.SouthAmerica:
                return 1.1f;
            case Market.Asia:
                return 2.0f;
            case Market.Europe:
                return 1.1f;
            case Market.Africa:
                return 1.6f;
            case Market.Australia:
                return 0.8f;
            case Market.Antarctica:
                return 0.6f;
            case Market.ExtraTerra:
                return 3.0f;
            case Market.Alien:
                return 4.0f;
            default:
                return 0f;
        }
    }
}
