using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Lobby : SharedResource<Lobby> {

    public float cost = 100000;
    public string description;
    public EffectSet effects = new EffectSet();

    public static Lobby[] LoadAll() {
        return Resources.LoadAll<Lobby>("Lobbies");
    }

    public static new Lobby Load(string name) {
        return Resources.Load("Lobbies/" + name) as Lobby;
    }

    public override string ToString() {
        return name;
    }
}
