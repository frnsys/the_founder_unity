using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Consultancy : ScriptableObject, IUnlockable {
    public float cost = 10;
    public string description;

    // The fallback research point value.
    public float baseResearch = 10;

    // Specializations. There are research point bonuses for these areas.
    public List<Industry> industries = new List<Industry>();
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Market> markets = new List<Market>();

    public int researchTime = 30; // seconds
}


