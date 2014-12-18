using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Infrastructure : Item {
    //public Store store = Store.Infrastructure;
    public Type type;

    public enum Type {
        Datacenter,
        Factory,
        Studio,
        Lab
    }
}

[System.Serializable]
public class Infrastructures : SerializableDictionary<Infrastructure.Type, int> {

    public Infrastructures() {
        // Initialize with 0 of each infrastructure type.
        foreach (Infrastructure.Type t in Enum.GetValues(typeof(Infrastructure.Type))) {
            Add(t, 0);
        }
    }

    public static bool operator >(Infrastructures left, Infrastructures right) {
        foreach(KeyValuePair<Infrastructure.Type, int> item in left) {
            if (item.Value < right[item.Key])
                return false;
        }
        return true;
    }

    public static bool operator <(Infrastructures left, Infrastructures right) {
        foreach(KeyValuePair<Infrastructure.Type, int> item in left) {
            if (item.Value > right[item.Key])
                return false;
        }
        return true;
    }
}
