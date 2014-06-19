using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {

    private int _level = 1;
    public int level {
        // Read-only.
        // The level should only be changed by GainExperience.
        get { return _level; }
    }
    public int xp = 0;
    private int nextLevelXp = 1000;

    public Office office;
    public float salary;

    public Stat happiness = new Stat("Happiness");
    public Stat productivity = new Stat("Productivity");

    public Stat charisma = new Stat("Charisma");
    public Stat creativity = new Stat("Creativity");
    public Stat cleverness = new Stat("Cleverness");

    public Stat xprate = new Stat("XP Rate", 25);

    void Update() {
    }

    public void GainExperience() {
        GainExperience((int)xprate.finalValue);
    }
    public void GainExperience(int xp_) {
        xp += xp_;

        // Level up!
        if (xp >= nextLevelXp) {
            _level++;

            // Formula for next level's xp requirement is:
            // (level^2 * 1000) + ((level - 1) * 1000)
            nextLevelXp = (int)(System.Math.Pow(level, 2) * 1000) + ((level-1) * 1000);
        }
    }
}


