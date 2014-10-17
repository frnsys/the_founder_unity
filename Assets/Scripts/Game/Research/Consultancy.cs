using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.Diagnostics;


public class Consultancy : ScriptableObject, IUnlockable {
    public float cost = 10;
    public string description;

    // The fallback research point value.
    public float baseResearch = 10;

    // Specializations. There are research point bonuses for these areas.
    public List<Industry> industries = new List<Industry>();
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Market> markets = new List<Market>();

    public int researchTime = 30000; // milliseconds

    private Stopwatch stopwatch = new Stopwatch();
    public float researchProgress {
        get {
            return stopwatch.ElapsedMilliseconds / (float)researchTime;
        }
    }


    static public event System.Action<Consultancy> ResearchCompleted;
    public void BeginResearch() {
        Timer timer = new Timer(researchTime);
        timer.Elapsed += delegate {
            // Research results
            timer.Stop();
            timer = null;

            stopwatch.Stop();
            stopwatch.Reset();

            // Trigger the ResearchCompleted event.
            if (ResearchCompleted != null) {
                ResearchCompleted(this);
            }
        };
        timer.Start();
        stopwatch.Start();
    }

}


