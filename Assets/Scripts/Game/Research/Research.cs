/*
 * Manages the player's research.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics; // for Stopwatch

public class ResearchManager : MonoBehaviour {
    private Stopwatch stopwatch = new Stopwatch();
    private Consultancy _consultancy;
    public Consultancy consultancy {
        get { return _consultancy; }
    }

    public float progress {
        get {
            if (_consultancy != null) {
                return stopwatch.ElapsedMilliseconds / (_consultancy.researchTime * 1000f);
            }
            return 0;
        }
    }
    public bool researching {
        get { return _consultancy != null; }
    }

    // NOWHERE in Unity's documentation does it say that
    // Invoke or StartCoroutine must be called from Update()
    // or Start(). In fact, the example in their docs shows
    // it being used in another method. Unity is atrocious.
    // So using this hacky method to do the research.
    void Update() {
        if (_consultancy != null && progress >= 1) {
            End();
        }
    }

    static public event System.Action<Consultancy> Completed;
    public void Begin(Consultancy c) {
        _consultancy = c;
        stopwatch.Start();
    }
    public void End() {
        stopwatch.Stop();
        stopwatch.Reset();

        // Trigger the Completed event.
        if (Completed != null) {
            Completed(_consultancy);
        }
        _consultancy = null;
    }

}


