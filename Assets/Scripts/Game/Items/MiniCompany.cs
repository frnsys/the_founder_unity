using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MiniCompany : SharedResource<MiniCompany> {

    public float cost = 100000;
    public string description;
    public Texture logo;
    public float revenue;
    public EffectSet effects = new EffectSet();

}
