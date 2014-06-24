using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameEventType {
    public enum Severity {
        MINOR,
        MAJOR,
        CATASTROPHIC
    }
    public enum Parity {
        GOOD,
        BAD
    }

    public float probability;

    public Dictionary<Severity, float> goodSeverityPs = new Dictionary<Severity, float>{
        {Severity.MINOR, 0.80f},
        {Severity.MAJOR, 0.18f},
        {Severity.CATASTROPHIC, 0.02f}
    };

    public Dictionary<Severity, float> badSeverityPs = new Dictionary<Severity, float>{
        {Severity.MINOR, 0.80f},
        {Severity.MAJOR, 0.18f},
        {Severity.CATASTROPHIC, 0.02f}
    };

    public Dictionary<Parity, float> parityPs = new Dictionary<Parity, float>{
        {Parity.GOOD, 0.5f},
        {Parity.BAD, 0.5f}
    };
}


