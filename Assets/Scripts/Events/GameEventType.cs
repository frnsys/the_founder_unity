using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    public string name;


    // I would like these to be private and have
    // getters which only return readonly copies but
    // read only dicts don't seem to be supported?
    public Dictionary<Severity, float> goodSeverities = new Dictionary<Severity, float>{
        {Severity.MINOR, 0.80f},
        {Severity.MAJOR, 0.18f},
        {Severity.CATASTROPHIC, 0.02f}
    };

    public Dictionary<Severity, float> badSeverities = new Dictionary<Severity, float>{
        {Severity.MINOR, 0.80f},
        {Severity.MAJOR, 0.18f},
        {Severity.CATASTROPHIC, 0.02f}
    };

    public Dictionary<Parity, float> parities = new Dictionary<Parity, float>{
        {Parity.GOOD, 0.5f},
        {Parity.BAD, 0.5f}
    };


    // Properties
    // ===============================================
    public float good {
        set { SetParity(Parity.GOOD, value); }
        get { return parities[Parity.GOOD]; }
    }

    public float bad {
        set { SetParity(Parity.BAD, value); }
        get { return parities[Parity.BAD]; }
    }

    public float good_minor {
        set { SetSeverity(Parity.GOOD, Severity.MINOR, value); }
        get { return goodSeverities[Severity.MINOR]; }
    }

    public float good_major {
        set { SetSeverity(Parity.GOOD, Severity.MAJOR, value); }
        get { return goodSeverities[Severity.MAJOR]; }
    }

    public float good_catastrophic {
        set { SetSeverity(Parity.GOOD, Severity.CATASTROPHIC, value); }
        get { return goodSeverities[Severity.CATASTROPHIC]; }
    }

    public float bad_minor {
        set { SetSeverity(Parity.BAD, Severity.MINOR, value); }
        get { return badSeverities[Severity.MINOR]; }
    }

    public float bad_major {
        set { SetSeverity(Parity.BAD, Severity.MAJOR, value); }
        get { return badSeverities[Severity.MAJOR]; }
    }

    public float bad_catastrophic {
        set { SetSeverity(Parity.BAD, Severity.CATASTROPHIC, value); }
        get { return badSeverities[Severity.CATASTROPHIC]; }
    }


    // Probability handling
    // ===============================================
    private void SetParity(Parity target, float value) {
        // Probability can't be higher than 1
        // or less than 0.
        value = Mathf.Clamp(value, 0, 1);

        parities[target] = value;
        if (target == Parity.BAD) {
            parities[Parity.GOOD] = 1 - value;
        } else {
            parities[Parity.BAD] = 1 - value;
        }
    }

    private void SetSeverity(Parity parity, Severity target, float value) {
        // Figure out which set of severity probabilities
        // we're modifying.
        Dictionary<Severity, float> probs;
        if (parity == Parity.GOOD) {
            probs = goodSeverities;
        } else {
            probs = badSeverities;
        }

        // Probability can't be higher than 1
        // or less than 0.
        value = Mathf.Clamp(value, 0, 1);

        // The mass which remaning probabilities needs to equal,
        // *after* the target probability is changed.
        float r_ = 1 - value;

        // Grab the remaining probabilities.
        IEnumerable<KeyValuePair<Severity, float>> probs_ = probs.Where(i => i.Key != target);
        KeyValuePair<Severity, float> x_kvp = probs_.ElementAt(0);
        KeyValuePair<Severity, float> y_kvp = probs_.ElementAt(1);
        float x_, y_;

        // This is solving the system:
        // x' + y' = r'
        // x' / y' = x/y
        // Where r' = 1 - value
        y_ = r_/((x_kvp.Value/y_kvp.Value) + 1);
        x_ = r_ - y_;

        // Update the probabilities.
        probs[x_kvp.Key] = x_;
        probs[y_kvp.Key] = y_;
        probs[target] = value;
    }



    public GameEventType(string name_, float probability_) {
        name = name_;
        probability = probability_;
    }
}


