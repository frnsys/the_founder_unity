using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Promo : SharedResource<Promo> {
    public Texture icon;
    public string description;
    public float cost;
    public int id;

    static public event System.Action<Promo> Completed;
    public void Complete() {
        if (Completed != null)
            Completed(this);
    }

    public int hype {
        get { return (int)(UnityEngine.Random.value * (cost/2000) + (cost/2000)); }
    }

    public static Promo[] LoadAll() {
        return Resources.LoadAll<Promo>("Promos");
    }
}
