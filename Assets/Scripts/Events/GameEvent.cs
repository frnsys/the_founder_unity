using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class GameEvent {
    public static List<GameEvent> Roll(List<GameEvent> candidateEvents) {
        List<GameEvent> theseAreHappening = new List<GameEvent>();

        foreach (GameEvent gameEvent in candidateEvents) {

            // Does this event happen?
            if (Random.value < gameEvent.probability.value) {
                theseAreHappening.Add(gameEvent);
            }

        }
        return theseAreHappening;
    }

    public string name;
    public Stat probability;

    public GameEvent(JSONClass prototype) {
        name = prototype["name"];
    }
    public GameEvent(string name_, float probability_) {
        name = name_;

        // Maximum probability is 1,
        // minimum is 0.
        probability_ = Mathf.Clamp(probability_, 0, 1);
        probability = new Stat("Probability", probability_);
    }
}


