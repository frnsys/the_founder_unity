/*
 * Manages the player's research.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResearchManager : MonoBehaviour {
    private GameData data;

    // The currently-hired consultancy.
    public Consultancy consultancy {
        get { return data.company.consultancy; }
    }

    // The currently-pursued discovery.
    public Discovery discovery {
        get { return data.discovery; }
    }

    // The accumulated research for the current discovery.
    public Research research {
        get { return data.research; }
    }
    public bool researching {
        get { return discovery != null; }
    }

    // Progress towards the discovery's completion.
    public float progress {
        get {
            if (discovery != null) {
                return research / discovery.requiredResearch;
            }
            return 0;
        }
    }

    // Accumulate research.
    public void Research() {
        if (consultancy && discovery) {
            data.research += consultancy.research;

            if (research >= discovery.requiredResearch) {
                EndResearch();
            }
        }
    }

    public bool HireConsultancy(Consultancy c) {
        // You pay the consultancy cost initially when hired, then repeated monthly.
        if (data.company.Pay(c.cost)) {
            data.company.consultancy = c;
            return true;
        }
        return false;
    }

    static public event System.Action<Discovery> Completed;
    public void BeginResearch(Discovery d) {
        data.discovery = d;
    }
    public void EndResearch() {
        // Trigger the Completed event.
        if (Completed != null) {
            Completed(discovery);
        }

        // Reset the discovery & accumulated research.
        data.discovery = null;
        data.research = new Research(0,0,0);
    }

    public void Load(GameData d) {
        data = d;
    }
}


