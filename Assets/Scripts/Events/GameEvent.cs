using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class GameEvent {
    private static JSONClass gameEvents;
    public static List<GameEvent> Roll(List<GameEventType> eventTypes) {
        // Load & cache the GameEvents reference.
        if (gameEvents == null) {
            TextAsset gE = Resources.Load("GameEvents") as TextAsset;
            gameEvents = JSON.Parse(gE.text).AsObject;
        }

        List<GameEvent> theseAreHappening = new List<GameEvent>();

        foreach (GameEventType eT in eventTypes) {

            // Does an event of this type happen?
            if (Random.value < eT.probability) {
                GameEventType.Parity selParity = GameEventType.Parity.GOOD;
                GameEventType.Severity selSeverity = GameEventType.Severity.MINOR;
                float roll;
                float minValue;

                // What parity?
                roll = Random.value;
                minValue = 1;
                foreach (KeyValuePair<GameEventType.Parity, float> entry in eT.parities) {
                    float prob = entry.Value;
                    if ( roll < prob && prob <= minValue) {
                        selParity = entry.Key;
                        minValue = prob;
                    }
                }

                Dictionary<GameEventType.Severity, float> severityPs = null;
                switch(selParity) {
                    case(GameEventType.Parity.GOOD):
                        severityPs = eT.goodSeverities;
                        break;
                    case(GameEventType.Parity.BAD):
                        severityPs = eT.badSeverities;
                        break;
                }

                // What severity?
                roll = Random.value;
                minValue = 1;
                foreach (KeyValuePair<GameEventType.Severity, float> entry in severityPs) {
                    float prob = entry.Value;
                    if ( roll < prob && prob <= minValue) {
                        selSeverity = entry.Key;
                        minValue = prob;
                    }
                }

                // TO DO this also needs to load event consequences,
                // actions and action consequences.

                // Get matching events.
                JSONArray matchingEvents = gameEvents[ eT.name ][ selParity.ToString() ][ selSeverity.ToString() ].AsArray;

                // Pick a random one.
                JSONClass selected = matchingEvents[Random.Range(0, matchingEvents.Count - 1)].AsObject;
                theseAreHappening.Add(new GameEvent(selected));

            }

        }
        return theseAreHappening;
    }

    public string name;
    public GameEvent(JSONClass prototype) {
        name = prototype["name"];
    }
    public GameEvent(string name_) {
        name = name_;
    }
}


