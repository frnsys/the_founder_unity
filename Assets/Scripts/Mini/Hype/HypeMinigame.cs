using UnityEngine;

public class HypeMinigame : Singleton<HypeMinigame> {
    public int pucks = 3;
    public HypePuck puck;

    private float hypeScore = 0;
    private float opinionScore = 0;

    void OnEnable() {
        HypeTarget.Scored += Scored;
        HypeTarget.Completed += Completed;
        HypePuck.Completed += Completed;
    }

    void OnDisable() {
        HypeTarget.Scored -= Scored;
        HypeTarget.Completed -= Completed;
        HypePuck.Completed -= Completed;
    }

    void Scored(float hypePoints, float opinionPoints) {
        hypeScore += hypePoints;
        opinionScore += opinionPoints;
    }

    void Completed() {
        if (pucks > 0) {
            pucks--;
            puck.Reset();
        } else {
            puck.gameObject.SetActive(false);
        }
        Debug.Log(string.Format("Scored {0} hype points!", hypeScore));
        Debug.Log(string.Format("Scored {0} opinion points!", opinionScore));
    }
}
