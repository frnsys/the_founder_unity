/*
 * A worker which contributes to the development of products.
 * "Worker" here is used abstractly - it can mean an employee, or a location, or a planet.
 * i.e. a worker is some _productive_ entity.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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

    public Worker(string name_, float happiness_, float productivity_, float charisma_, 
        float creativity_, float cleverness_
    ) {
        name         = name_;
        happiness    = new Stat("Happiness", happiness_);
        productivity = new Stat("Productivity", productivity_);
        charisma     = new Stat("Charisma", charisma_);
        creativity   = new Stat("Creativity", creativity_);
        cleverness   = new Stat("Cleverness", cleverness_);
    }

    void Start() {
        //levels = this.gameObject.GetComponent<Levels>();
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


