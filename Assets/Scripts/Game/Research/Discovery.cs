using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Discovery : ScriptableObject, IUnlockable {
    public string description;

    public Research requiredResearch = new Research(100,100,100);

    public EffectSet effects = new EffectSet();
}


