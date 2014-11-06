/*
 * Manages the player's research.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResearchManager : MonoBehaviour {
    // The currently-hired consultancy.
    public Consultancy consultancy;

    // The currently-pursued discovery.
    private Discovery discovery_;
    public Discovery discovery {
        get { return discovery_; }
    }

    // The accumulated research for the current discovery.
    private Research research_ = new Research(0,0,0);
    public Research research {
        get { return research_; }
    }
    public bool researching {
        get { return discovery != null; }
    }

    // Progress towards the discovery's completion.
    public float progress {
        get {
            if (discovery != null) {
                return research_ / discovery_.requiredResearch;
            }
            return 0;
        }
    }

    // Accumulate research.
    public void Research() {
        if (consultancy && discovery_) {
            research_ += consultancy.research;

            if (research_ >= discovery_.requiredResearch) {
                EndResearch();
            }
        }
    }

    static public event System.Action<Discovery> Completed;
    public void BeginResearch(Discovery d) {
        discovery_ = d;
    }
    public void EndResearch() {
        // Trigger the Completed event.
        if (Completed != null) {
            Completed(discovery_);
        }

        // Reset the discovery & accumulated research.
        discovery_ = null;
        research_ = new Research(0,0,0);
    }

}


