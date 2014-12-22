/*
 * Manages events, in particular, events with unique effects (beyond those supported by EffectSet).
 */

using UnityEngine;

public class EventManager : MonoBehaviour {
    private GameData data;

    public void Load(GameData d) {
        data = d;
    }

    public void IncreaseMaxProductTypes() {
        data.maxProductTypes = 2;
    }
}
