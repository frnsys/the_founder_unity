/*
 * Manages the player's research.
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ResearchManager : MonoBehaviour {
    private GameData data;

    // The currently-pursued technology.
    public Technology technology {
        get { return data.technology; }
    }

    // The accumulated research for the current technology.
    public float research {
        get { return data.research; }
    }
    public bool researching {
        get { return technology != null; }
    }

    // Progress towards the technology's completion.
    public float progress {
        get {
            return technology != null ? research / technology.requiredResearch : 0;
        }
    }

    // Accumulate research.
    public void Research() {
        if (technology) {
            // TO DO invested cash shouldn't have as simple a relationship to research.
            data.research += data.company.research.value + data.company.researchInvestment/1000;

            // TESTING purposes
            data.research += 100000;

            if (research >= technology.requiredResearch) {
                EndResearch();
            }
        }
    }

    static public event System.Action<Technology> Completed;
    public void BeginResearch(Technology d) {
        // Research resets for new technologies.
        data.research = 0;
        data.technology = d;
    }
    public void EndResearch() {
        // Trigger the Completed event.
        if (Completed != null) {
            data.company.technologies.Add(technology);
            Completed(technology);
        }

        // Reset the technology & accumulated research.
        data.technology = null;
        data.research = 0;
    }

    public void Load(GameData d) {
        data = d;
    }

    public IEnumerable<Technology> AvailableTechnologies() {
        return Technology.LoadAll().Where(t => t.isAvailable(data.company));
    }
}


