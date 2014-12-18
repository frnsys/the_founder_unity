/*
 * Manages the player's research.
 */

using UnityEngine;
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
            if (technology != null) {
                return research / technology.requiredResearch;
            }
            return 0;
        }
    }

    // Accumulate research.
    public void Research() {
        if (technology) {
            // TO DO invested cash shouldn't have as simple a relationship to research.
            data.research += data.company.research.value + data.company.researchCash/1000;

            if (research >= technology.requiredResearch) {
                EndResearch();
            }
        }
    }

    static public event System.Action<Technology> Completed;
    public void BeginResearch(Technology d) {
        data.technology = d;
    }
    public void EndResearch() {
        // Trigger the Completed event.
        if (Completed != null) {
            Completed(technology);
        }

        // Reset the technology & accumulated research.
        data.technology = null;
        data.research = 0;
    }

    public void Load(GameData d) {
        data = d;
    }
}


