using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameEvent {
    public string name;

    public static GameEvent Roll(float baseProbability, List<GameEventType> eventTypes) {
        // Does an event happen at all?
        if (Random.value < baseProbability) {
            GameEventType selType = null;
            GameEventType.Parity selParity = GameEventType.Parity.GOOD;
            GameEventType.Severity selSeverity = GameEventType.Severity.MINOR;
            float roll;
            float minValue;

            // ROLL FOR EVENT TYPE.
            // Find the event type who's probability range
            // the roll falls into.
            roll = Random.value;
            minValue = 1;
            foreach (GameEventType eventType in eventTypes) {
                float prob = eventType.probability;
                if ( roll < prob && prob <= minValue) {
                    selType = eventType;
                    minValue = prob;
                }
            }

            // ROLL FOR PARITY.
            roll = Random.value;
            minValue = 1;
            foreach (KeyValuePair<GameEventType.Parity, float> entry in selType.parityPs) {
                float prob = entry.Value;
                if ( roll < prob && prob <= minValue) {
                    selParity = entry.Key;
                    minValue = prob;
                }
            }

            Dictionary<GameEventType.Severity, float> severityPs = null;
            switch(selParity) {
                case(GameEventType.Parity.GOOD):
                    severityPs = selType.goodSeverityPs;
                    break;
                case(GameEventType.Parity.BAD):
                    severityPs = selType.badSeverityPs;
                    break;
            }

            // ROLL FOR SEVERITY.
            roll = Random.value;
            minValue = 1;
            foreach (KeyValuePair<GameEventType.Severity, float> entry in severityPs) {
                float prob = entry.Value;
                if ( roll < prob && prob <= minValue) {
                    selSeverity = entry.Key;
                    minValue = prob;
                }
            }

            // SELECT A MATCHING EVENT.
            // To do
        }
        return null;
    }
}


