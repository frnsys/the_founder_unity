/*
 * Companies managed by AI for you to compete against.
 *
 * The AI company has a minimal set of things it actually does
 * to simulate competition with the player.
 * Everything else is faked.
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AICompany : ScriptableObject {
    public string slogan;
    public string description;
    public EffectSet bonuses;
    public Worker founder;
    public int sizeLimit = 5;
    public bool disabled = false;

    public int designSkill;
    public int engineeringSkill;
    public int marketingSkill;

    // To easily keep track of all AI companies.
    public static List<AAICompany> all = new List<AAICompany>();

    public static List<AAICompany> LoadAll() {
        // Load companies as _copies_ so any changes don't get saved to the actual resources.
        return Resources.LoadAll<AICompany>("Companies").ToList().Select(c => {
            return new AAICompany(c);
        }).ToList();
    }

    // Convenience method for locating the in-game version
    // of an AICompany from an asset version.
    public static AAICompany Find(AICompany aic) {
        return Find(aic.name);
    }
    public static AAICompany Find(string name) {
        return all.Where(a => a.name == name).First();
    }

    // Call `Init()` if creating a new AICompany from scratch.
    public new AICompany Init() {
        // Initialize stuff.
        bonuses = new EffectSet();
        return this;
    }
}
