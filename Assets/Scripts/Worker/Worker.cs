using UnityEngine;
using System.Collections;

public enum WorkerType {
    EMPLOYEE,
    LOCATION,
    PLANET,
    STARSYSTEM
}

public class Worker : MonoBehaviour {

    private Levels levels;

    public WorkerType type;

    public float salary;

    public Stat happiness = new Stat("Happiness");
    public Stat productivity = new Stat("Productivity");
    public Stat charisma = new Stat("Charisma");
    public Stat creativity = new Stat("Creativity");
    public Stat cleverness = new Stat("Cleverness");

    void Start() {
        levels = this.gameObject.GetComponent<Levels>();
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
        print("Leveled");
    }
}


