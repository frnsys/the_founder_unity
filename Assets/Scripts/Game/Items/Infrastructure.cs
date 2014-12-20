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
public class InfrastructureDict : SerializableDictionary<Infrastructure.Type, int> {

    public InfrastructureDict() {
        // Initialize with 0 of each infrastructure type.
        foreach (Infrastructure.Type t in Enum.GetValues(typeof(Infrastructure.Type))) {
            Add(t, 0);
        }
    }

    public override string ToString() {
        string repr = "";
        foreach(KeyValuePair<Infrastructure.Type, int> item in this) {
            repr += item.Key.ToString() + ":" + item.Value.ToString() + " ";
        }
        return repr;
    }

    public bool Equals(InfrastructureDict right) {
        foreach(KeyValuePair<Infrastructure.Type, int> item in this) {
            if (item.Value != right[item.Key])
                return false;
        }
        return true;
    }

    public static bool operator >(InfrastructureDict left, InfrastructureDict right) {
        foreach(KeyValuePair<Infrastructure.Type, int> item in left) {
            if (item.Value < right[item.Key])
                return false;
        }
        return true;
    }

    public static bool operator <(InfrastructureDict left, InfrastructureDict right) {
        foreach(KeyValuePair<Infrastructure.Type, int> item in left) {
            if (item.Value > right[item.Key])
                return false;
        }
        return true;
    }

    public static InfrastructureDict operator +(InfrastructureDict left, InfrastructureDict right) {
        InfrastructureDict result = new InfrastructureDict();
        foreach(KeyValuePair<Infrastructure.Type, int> item in left) {
            result[item.Key] = item.Value + right[item.Key];
        }
        return result;
    }

    public static InfrastructureDict operator -(InfrastructureDict left, InfrastructureDict right) {
        InfrastructureDict result = new InfrastructureDict();
        foreach(KeyValuePair<Infrastructure.Type, int> item in left) {
            result[item.Key] = item.Value - right[item.Key];
        }
        return result;
    }
}
