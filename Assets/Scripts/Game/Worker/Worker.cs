/*
 * A worker which contributes to the development of products.
 * "Worker" here is used abstractly - it can mean an employee, or a location, or a planet.
 * i.e. a worker is some _productive_ entity.
 */

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Worker : HasStats {
    public enum Tier {
        Employee,
        Location,
        Planet,
        StarSystem
    }
    public Tier tier;

    public static List<Worker> LoadAll(Tier tier) {
        return new List<Worker>(Resources.LoadAll<Worker>("Workers/" + tier + "s"));
    }

    private Levels levels;


    public float salary;
    public string bio;

    public Stat happiness;
    public Stat productivity;
    public Stat charisma;
    public Stat creativity;
    public Stat cleverness;

    void Start() {
        Init("Default Worker");
    }
    public void Init(string name_) {
        name         = name_;
        salary       = 0;
        happiness    = new Stat("Happiness",    0);
        productivity = new Stat("Productivity", 0);
        charisma     = new Stat("Charisma",     0);
        creativity   = new Stat("Creativity",   0);
        cleverness   = new Stat("Cleverness",   0);
        //levels     = this.gameObject.GetComponent<Levels>();
    }

    public void OnEnable() {
        if (levels) {
            levels.LevelUp += LeveledUp;
        }
    }
    public void OnDisable() {
        if (levels) {
            levels.LevelUp -= LeveledUp;
        }
    }

    void LeveledUp(int level) {
        //print("Leveled");
    }

    public void ApplyItem(Item item) {
        ApplyBuffs(item.effects.workers);
    }

    public void RemoveItem(Item item) {
        RemoveBuffs(item.effects.workers);
    }

    public override Stat StatByName(string name) {
        switch (name) {
            case "Happiness":
                return happiness;
            case "Productivity":
                return productivity;
            case "Charisma":
                return charisma;
            case "Creativity":
                return creativity;
            case "Cleverness":
                return cleverness;
            default:
                return null;
        }
    }
}


