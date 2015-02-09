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

    // sketch
    public static void CalculateMarketShares(List<Company> companies) {
        // Calculate the aggregate market share for every active product.
        // The market share is the market size the product has captured.
        // For instance, if a product is in the Asia and Europe markets,
        // with total sizes of 2.0 and 1.1 respectively, then it might have
        // a market share of 2.2 out of the total 3.1.

        // Calculate the market scores
        // and reset the market shares.
        foreach (Company c in companies) {
            foreach (Product p in c.activeProducts) {
                p.marketScore = c.publicity.value + c.opinion.value + p.marketing.value + p.design.value/2 + p.engineering.value/3;
                p.marketShare = 0;
            }
        }

        foreach (MarketManager.Market m in MarketManager.Markets) {
            float marketSize = SizeForMarket(m);

            // Keep track of combo totals.
            Dictionary<string, float> comboTotals = new Dictionary<string, float>();

            // Keep track of products by combo.
            Dictionary<string, List<Product>> comboProducts = new Dictionary<string, List<Product>>();

            foreach (Company c in companies.Where(x => x.markets.Contains(m))) {
                foreach (Product p in c.activeProducts) {
                    if (!comboTotals.ContainsKey(p.comboID)) {
                        comboTotals[p.comboID] = 0;
                        comboProducts[p.comboID] = new List<Product>();
                    }
                    comboTotals[p.comboID] += p.marketScore;
                    comboProducts[p.comboID].Add(p);
                }
            }

            foreach(KeyValuePair<string, List<Product>> i in comboProducts) {
                foreach (Product p in i.Value) {
                    p.marketShare += p.marketScore/comboTotals[i.Key] * marketSize;
                }
            }
        }
    }
}
