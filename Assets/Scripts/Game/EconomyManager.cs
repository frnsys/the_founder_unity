using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class EconomyManager : Singleton<EconomyManager> {
    private GameData data;
    private float economyChangeProbability;

    // Disable the constructor.
    protected EconomyManager() {}

    public void Load(GameData d) {
        data = d;
        UpdateEconomyChangeProbability();

        StartCoroutine(EconomyCycle());
    }

    void UpdateEconomyChangeProbability() {
        switch(data.economy) {
            case Economy.Depression:
                economyChangeProbability = 0.2f;
                break;
            case Economy.Recession:
                economyChangeProbability = 0.1f;
                break;
            case Economy.Neutral:
                economyChangeProbability = 0.05f;
                break;
            case Economy.Expansion:
                economyChangeProbability = 0.16f;
                break;
        }
    }

    void ChangeEconomy() {
        float roll = Random.value;

        Economy newEconomy;
        string eventName = null;
        if (roll < 1) {
            newEconomy = Economy.Depression;
            eventName = "Economic Depression";
        } else if (roll < 0.15) {
            newEconomy = Economy.Recession;
            eventName = "Economic Recession";
        } else if (roll < 0.20) {
            newEconomy = Economy.Expansion;
            eventName = "Economic Expansion";
        } else {
            newEconomy = Economy.Neutral;
        }

        if (newEconomy != data.economy) {
            data.economy = newEconomy;

            // Economic recovery is a special case.
            if ((data.economy == Economy.Recession || data.economy == Economy.Depression) && (newEconomy == Economy.Expansion || newEconomy == Economy.Neutral)) {
                eventName = "Economic Recovery";
            }

            if (eventName != null) {
                GameEvent ev = GameEvent.LoadSpecialEvent(eventName);
                GameEvent.Trigger(ev);
            }
            UpdateEconomyChangeProbability();
        }
    }

    public float economyMultiplier {
        get { return (int)data.economy / 3f; }
    }

    IEnumerator EconomyCycle() {
        yield return new WaitForSeconds(10);
        while (true) {
            // See if the economy changes.
            if (Random.value < economyChangeProbability) {
                ChangeEconomy();
            }
            yield return new WaitForSeconds(72);
        }
    }
}
