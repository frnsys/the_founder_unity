using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Infrastructure : SerializableDictionary<Infrastructure.Type, int> {
    public enum Type {
        Datacenter,
        Factory,
        Studio,
        Lab
    }

    public static Type[] Types {
        get { return Enum.GetValues(typeof(Type)) as Type[]; }
    }

    // Create an infrastructure consisting of 1 unit of 1 type.
    public static Infrastructure ForType(Type t) {
        Infrastructure inf = new Infrastructure();
        inf[t] = 1;
        return inf;
    }

    public Infrastructure() {
        // Initialize with 0 of each infrastructure type.
        foreach (Type t in Enum.GetValues(typeof(Type))) {
            Add(t, 0);
        }
    }

    public static string Plural(Type t) {
        switch (t) {
            case Type.Datacenter:
                return "datacenters";
            case Type.Factory:
                return "factories";
            case Type.Studio:
                return "studios";
            case Type.Lab:
                return "labs";
        }
        return "infrastructures";
    }

    // Each piece of infrastructure costs 1000.
    public static int baseCost {
        get { return 1000; }
    }

    // Returns the cost for this set of infrastructure.
    public int cost {
        get {
            int totalCost = 0;
            foreach(KeyValuePair<Type, int> item in this) {
                totalCost += (int)(item.Value * baseCost * (GameManager.Instance.infrastructureCostMultiplier[item.Key]/100f));
            }
            return totalCost;
        }
    }

    public bool isEmpty {
        get {
            foreach(KeyValuePair<Type, int> item in this) {
                if (item.Value > 0)
                    return false;
            }
            return true;
        }
    }

    public override string ToString() {
        string repr = "";
        foreach(KeyValuePair<Type, int> item in this) {
            if (item.Value > 0)
                repr += item.Key.ToString() + ":" + item.Value.ToString() + " ";
        }
        return repr;
    }

    public bool Equals(Infrastructure right) {
        foreach(KeyValuePair<Type, int> item in this) {
            if (item.Value != right[item.Key])
                return false;
        }
        return true;
    }

    // Gets the intersection of two infrastructures.
    public Infrastructure Intersection(Infrastructure right) {
        Infrastructure result = new Infrastructure();
        foreach(KeyValuePair<Type, int> item in this) {
            result[item.Key] = Math.Min(item.Value, right[item.Key]);
        }
        return result;
    }

    // Returns true if _any_ values in this is greater than the corresponding value in `right`.
    public bool AnyGreater(Infrastructure right) {
        foreach(KeyValuePair<Type, int> item in this) {
            if (item.Value > right[item.Key])
                return true;
        }
        return false;
    }

    public static bool operator <=(Infrastructure left, Infrastructure right) {
        foreach(KeyValuePair<Type, int> item in left) {
            if (item.Value > right[item.Key])
                return false;
        }
        return true;
    }

    public static bool operator >=(Infrastructure left, Infrastructure right) {
        foreach(KeyValuePair<Type, int> item in left) {
            if (item.Value < right[item.Key])
                return false;
        }
        return true;
    }

    public static bool operator >(Infrastructure left, Infrastructure right) {
        foreach(KeyValuePair<Type, int> item in left) {
            if (item.Value <= right[item.Key])
                return false;
        }
        return true;
    }

    public static bool operator <(Infrastructure left, Infrastructure right) {
        foreach(KeyValuePair<Type, int> item in left) {
            if (item.Value >= right[item.Key])
                return false;
        }
        return true;
    }

    public static Infrastructure operator +(Infrastructure left, Infrastructure right) {
        Infrastructure result = new Infrastructure();
        foreach(KeyValuePair<Type, int> item in left) {
            result[item.Key] = item.Value + right[item.Key];
        }
        return result;
    }

    public static Infrastructure operator -(Infrastructure left, Infrastructure right) {
        Infrastructure result = new Infrastructure();
        foreach(KeyValuePair<Type, int> item in left) {
            result[item.Key] = item.Value - right[item.Key];

            // The smallest is 0, no negatives.
            if (result[item.Key] < 0)
                result[item.Key] = 0;
        }
        return result;
    }
}
