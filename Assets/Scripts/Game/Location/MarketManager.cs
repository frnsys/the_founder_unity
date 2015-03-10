/*
 * Manages markets.
 */

using UnityEngine;
using System;
using System.Linq;
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

    public static Market[] Markets {
        get { return Enum.GetValues(typeof(Market)) as Market[]; }
    }

    // Market sizes are meant to be a rough representation of a market's population,
    // or, more accurately, their capacity for consumption.
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

    public static string NameForMarket(Market m) {
        switch (m) {
            case Market.NorthAmerica:
                return "North America";
            case Market.SouthAmerica:
                return "South America";
            case Market.Asia:
                return "Asia";
            case Market.Europe:
                return "Europe";
            case Market.Africa:
                return "Africa";
            case Market.Australia:
                return "Australia";
            case Market.Antarctica:
                return "Antractica";
            case Market.ExtraTerra:
                return "Space Colony";
            case Market.Alien:
                return "Extraterrestrial";
            default:
                return "Global";
        }
    }

    private static Dictionary<Market, float> marketLocations_;
    public static Dictionary<Market, float> marketLocations {
        get {
            if (marketLocations_ == null) {
                marketLocations_ = new Dictionary<Market, float>();

                foreach (Market m in Markets) {
                    marketLocations_[m] = 0;
                }

                foreach (Location l in Location.LoadAll()) {
                    marketLocations_[l.market] += 1;
                }
            }
            return marketLocations_;
        }
    }
    private static float totalMarketSize_;
    public static float totalMarketSize {
        get {
            if (totalMarketSize_ == 0) {
                totalMarketSize_ = Markets.Sum(m => SizeForMarket(m));
            }
            return totalMarketSize_;
        }
    }
}
