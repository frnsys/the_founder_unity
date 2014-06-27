using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WorkerType {
    EMPLOYEE,
    LOCATION,
    PLANET,
    STARSYSTEM
}

public class Worker {

    private Levels levels;

    public WorkerType type;

    public float salary;

    public Stat happiness;
    public Stat productivity;
    public Stat charisma;
    public Stat creativity;
    public Stat cleverness;

    public Worker(float happiness_, float productivity_, float charisma_, 
        float creativity_, float cleverness_
    ) {
        happiness = new Stat("Happiness", happiness_);
        productivity = new Stat("Productivity", productivity_);
        charisma = new Stat("Charisma", charisma_);
        creativity = new Stat("Creativity", creativity_);
        cleverness = new Stat("Cleverness", cleverness_);
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
    }
}


