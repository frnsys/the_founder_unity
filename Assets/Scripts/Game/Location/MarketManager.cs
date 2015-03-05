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

    public static void CalculateMarketShares(Company playerCompany, List<AICompany> aiCompanies) {
        // Calculate the aggregate market share for every active product.
        // The market share is the market size the product has captured.
        // For instance, if a product is in the Asia and Europe markets,
        // with total sizes of 2.0 and 1.1 respectively, then it might have
        // a market share of 2.2 out of the total 3.1.

        // Keep track of combo totals.
        // This is reset for each market.
        Dictionary<string, float> comboTotals = new Dictionary<string, float>();

        // Calculate the market scores
        // and reset the market shares
        // for the player company only (AI companies's don't matter in the end)
        foreach (Product p in playerCompany.activeProducts) {
            p.marketScore = MarketScore(p, playerCompany);
            p.marketShare = 0;

            // Initialize the combo totals.
            comboTotals[p.comboID] = p.marketScore;
        }

        // Get only AI active products which match the player company's active products.
        List<Product> aiProducts = aiCompanies.SelectMany(c => c.activeProducts).Where(p => comboTotals.ContainsKey(p.comboID)).ToList();

        foreach (MarketManager.Market m in MarketManager.Markets) {
            float marketSize = SizeForMarket(m);

            // We assume the AI companies are already in every market.
            foreach (Product p in aiProducts) {
                comboTotals[p.comboID] += p.marketScore;
            }

            // Again, we only care about market shares for the player company.
            foreach (Product p in playerCompany.activeProducts) {
                p.marketShare += p.marketScore/comboTotals[p.comboID] * marketSize;

                // Reset the combo totals.
                comboTotals[p.comboID] = p.marketScore;
            }
        }
    }

    private static float MarketScore(Product p, Company c) {
        return c.publicity.value + c.opinion.value + p.design.value + p.marketing.value + p.engineering.value;
    }
}
