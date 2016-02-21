using UnityEngine;
using System;
using System.Collections;


public class ResourceTile : OwnedTile {
    public enum Type {
        Factory,
        Datacenter,
        Lab,
        Studio
    }
    public Type type;

    public void RandomizeType() {
        Array values = Enum.GetValues(typeof(Type));
        System.Random random = new System.Random();
        type = (Type)values.GetValue(random.Next(values.Length));
    }
}
