using UnityEngine;

[System.Serializable]
public class Infrastructure : Item {
    public Store store = Store.Infrastructure;
    public Type type;

    public enum Type {
        Datacenter,
        Factory,
        Studio,
        Lab
    }
}
