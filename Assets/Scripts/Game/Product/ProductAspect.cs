/*
 * An abstract class from which specific product aspects
 * (i.e. ProductType, Industry, & Market) are derived.
 */

using UnityEngine;
using System.Collections;

public abstract class ProductAspect : ScriptableObject {
    // Product points.
    // Each aspect requires a number of product points from a company.
    public int points;

    public string description;

    public override string ToString() {
        return name;
    }
}
