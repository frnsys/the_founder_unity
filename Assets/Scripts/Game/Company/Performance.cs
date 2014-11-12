using UnityEngine;

// Subclass these generic types so that they serialize.
[System.Serializable]
public class PerformanceDict : SerializableDictionary<string, float> {
    public override string ToString() {
        string str = "";
        foreach (string key in Keys) {
            str += key + ":" + this[key].ToString() + ", ";
        }
        return str.Substring(0, str.Length - 2);
    }
}

[System.Serializable]
public class PerformanceHistory : FixedSizeQueue<PerformanceDict> {

    public PerformanceHistory(int size) : base(size) {
        Size = size;
    }

    public override string ToString() {
        string str = "";
        foreach (PerformanceDict d in this) {
            str += d.ToString() + " | ";
        }
        return str.Substring(0, str.Length - 3);
    }
}
