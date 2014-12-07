/*
 * Consultancies are the entities which conduct research for a company.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Consultancy : ScriptableObject {
    public string description;

    // Monthly cost.
    public float cost = 10;

    // The amount of research the consultancy outputs.
    // There's a possibility to model specialization here:
    // Technical consultancies give bonus cleverness.
    // Design consultancies give bonus creativity.
    // Management consultancies give bonus charisma.
    public Research research = new Research(10,10,10);
}


