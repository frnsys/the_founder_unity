using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Recruitment : SharedResource<Recruitment> {
    public Texture icon;
    public float cost;
    public float targetScore;
    public string description;
    public bool robots;

    static public event System.Action<Recruitment> Completed;
    public void Develop() {
        if (Completed != null)
            Completed(this);
    }
}
