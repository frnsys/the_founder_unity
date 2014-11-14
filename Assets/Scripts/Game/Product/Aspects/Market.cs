using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Market : ProductAspect {
    public static Market Load(string name) {
        return Resources.Load("Products/Markets/" + name) as Market;
    }

    public static List<Market> LoadAll() {
        return new List<Market>(Resources.LoadAll<Market>("Products/Markets"));
    }
}
