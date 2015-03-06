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

    public static Dictionary<Market, float> marketLocations;
    public static float totalMarketSize = 0;

    public static void CalculateMarketShares(Company playerCompany, List<AICompany> aiCompanies) {
        // Calculate the aggregate market share for every active product.
        // The market share is the market size the product has captured.
        // For instance, if a product is in the Asia and Europe markets,
        // with total sizes of 2.0 and 1.1 respectively, then it might have
        // a market share of 2.2 out of the total 3.1.

        // Count how many locations are in each market.
        if (marketLocations == null) {
            marketLocations = new Dictionary<Market, float>();
            totalMarketSize = 0;

            foreach (Market m in Markets) {
                marketLocations[m] = 0;
                totalMarketSize += SizeForMarket(m);
            }

            foreach (Location l in Location.LoadAll()) {
                marketLocations[l.market] += 1;
            }

        }

        // Keep track of combo totals.
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
        // We assume the AI companies are already in every market.
        foreach (Product p in aiCompanies.SelectMany(c => c.activeProducts).Where(p => comboTotals.ContainsKey(p.comboID))) {
            comboTotals[p.comboID] += p.marketScore;
        }

        foreach (Market m in Markets) {
            float marketSize = SizeForMarket(m);
            float marketShare = playerCompany.LocationsForMarket(m)/marketLocations[m];

            // Again, we only care about market shares for the player company.
            foreach (Product p in playerCompany.activeProducts) {
                p.marketShare += p.marketScore/comboTotals[p.comboID] * marketShare * marketSize;
            }
        }

        // Normalize the market shares.
        foreach (Product p in playerCompany.activeProducts) {
            p.marketShare /= totalMarketSize;
        }
    }

    private static float MarketScore(Product p, Company c) {
        return c.publicity.value + c.opinion.value + p.design.value + p.marketing.value + p.engineering.value;
    }
}
