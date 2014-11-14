using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Industry : ProductAspect {
    public static Industry Load(string name) {
        return Resources.Load("Products/Industries/" + name) as Industry;
    }

    public static List<Industry> LoadAll() {
        return new List<Industry>(Resources.LoadAll<Industry>("Products/Industries"));
    }
}
