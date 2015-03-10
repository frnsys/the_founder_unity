using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Promo : TemplateResource<Promo> {
    public Texture icon;
    public string description;
    public float cost;
    public GameObject level;

    static public event System.Action<Promo> Completed;
    public void Develop() {
        if (Completed != null)
            Completed(this);
    }
}
