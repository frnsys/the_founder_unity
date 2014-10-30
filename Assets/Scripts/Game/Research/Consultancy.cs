using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Consultancy : ScriptableObject, IUnlockable {
    public float cost = 10;
    public string description;

    // Specializations.
    // Technical consultancies give bonus cleverness.
    // Design consultancies give bonus creativity.
    // Management consultancies give bonus charisma.
    public float research = new Research(10,10,10);
}


