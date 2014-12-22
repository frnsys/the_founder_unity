/*
 * A worker which contributes to the development of products.
 * "Worker" here is used abstractly - it can mean an employee, or a location, or a planet.
 * i.e. a worker is some _productive_ entity.
 */

using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class Worker : HasStats {
    public static List<Worker> LoadAll() {
        // Load workers as _copies_ so any changes don't get saved to the actual resources.
        return Resources.LoadAll<Worker>("Workers").ToList().Select(w => Instantiate(w) as Worker).ToList();
    }

    private Levels levels;

    // Each worker has their own style!
    public Texture texture;

    public float salary;
    public string bio;
    public float baseMinSalary;
    public float minSalary {
        get {
            // If the employee is currently hired, i.e. has a salary > 0,
            // their minimum acceptable salary depends on their happiness at their current company.
            // If happiness is below 5, the employee will actually accept a lower salary to move.
            if (salary > 0)
                return salary * (1 + (happiness.value - 5)/10);
            return baseMinSalary;
        }
    }

    // How many weeks the worker is off the job market for.
    // Recent offers the player has made.
    public int offMarketTime;
    public int recentPlayerOffers;

    public Stat happiness;
    public Stat productivity;
    public Stat charisma;
    public Stat creativity;
    public Stat cleverness;

    void Start() {
        Init("Default Worker");
    }
    public void Init(string name_) {
        name          = name_;
        salary        = 0;
        baseMinSalary = 30000;
        happiness     = new Stat("Happiness",    0);
        productivity  = new Stat("Productivity", 0);
        charisma      = new Stat("Charisma",     0);
        creativity    = new Stat("Creativity",   0);
        cleverness    = new Stat("Cleverness",   0);

        offMarketTime = 0;
        recentPlayerOffers = 0;
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


