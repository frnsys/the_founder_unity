using UnityEngine;
using System.Collections;

public class Levels : MonoBehaviour {

    public event System.Action<int> LevelUp;

    private int _level = 1;
    public int level {
        // Read-only.
        // The level should only be changed by GainExperience.
        get { return _level; }
    }
    public int xp = 0;
    public int xpRate = 25;
    private int nextLevelXp = 1000;


    public void GainExperience() {
        GainExperience(xpRate);
    }
    public void GainExperience(int xp_) {
        xp += xp_;

        // Level up!
        if (xp >= nextLevelXp) {
            _level++;

            // Broadcast LevelUp event.
            if ( LevelUp != null ) {
                LevelUp(_level);
            }

            // Formula for next level's xp requirement is:
            // (level^2 * 1000) + ((level - 1) * 1000)
            nextLevelXp = (int)(System.Math.Pow(level, 2) * 1000) + ((level-1) * 1000);
        }
    }
}


