/*
 * Manages the player's research.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResearchManager : MonoBehaviour {
    public Consultancy consultancy;
    private Discovery discovery;
    private Research research;

    public float progress {
        get {
            if (discovery != null) {
                return research / discovery.research;
            }
            return 0;
        }
    }

    public void Research() {
        research += consultancy.research;

        if (research >= discovery.research) {
            EndResearch();
        }
    }

    static public event System.Action<Discovery> Completed;
    public void BeginResearch(Discovery d) {
        discovery = d;
    }
    public void EndResearch() {
        // Trigger the Completed event.
        if (Completed != null) {
            Completed(discovery);
        }

        // Reset.
        discovery = null;
        research = new Research(0,0,0);
    }

}


