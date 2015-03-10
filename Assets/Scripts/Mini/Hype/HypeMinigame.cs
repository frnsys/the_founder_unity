using UnityEngine;

public class HypeMinigame : Singleton<HypeMinigame> {
    public int pucks = 3;
    public HypePuck puck;
    public UILabel pitchesLabel;
    public UILabel scoreLabel;
    public Transform gameBoard;
    public Promo promo;
    public Texture[] blebTextures;

    private float hypeScore = 0;
    private float opinionScore = 0;

    private GameObject level;
    public void Setup(Promo promo) {
        promo = promo;
        level = Instantiate(promo.level) as GameObject;
        level.transform.parent = gameBoard;

        // This is a hack so that the mentor message
        // doesn't have to deal with layering issues with the blebs.
        level.SetActive(false);
        Invoke("StartGame", 0.5f);
    }

    void StartGame() {
        level.SetActive(true);
    }

    void OnEnable() {
        HypeTarget.Scored += Scored;
        HypeTarget.Completed += Completed;
        HypePuck.Completed += Completed;
        UpdateLabels();
    }

    void OnDisable() {
        HypeTarget.Scored -= Scored;
        HypeTarget.Completed -= Completed;
        HypePuck.Completed -= Completed;
    }

    void Scored(float hypePoints, float opinionPoints) {
        hypeScore += hypePoints;
        opinionScore += opinionPoints;
        UpdateLabels();
    }

    void Completed() {
        if (pucks > 0) {
            pucks--;
            puck.Reset();
        } else {
            // Game Over
            puck.gameObject.SetActive(false);
            EndGame();
        }
        UpdateLabels();
    }

    void UpdateLabels() {
        pitchesLabel.text = string.Format("Pitches: {0}", pucks);
        scoreLabel.text = string.Format("Hype: {0}, PR: {1}", hypeScore, opinionScore);
    }

    void EndGame() {
        int numPeople = (int)(Mathf.Pow(hypeScore, 2) * 1000 * (Random.value + 0.75f));

        GameEvent ev = GameEvent.LoadNoticeEvent("Promo Failure");
        if (hypeScore + opinionScore >= 8) {
            ev = GameEvent.LoadNoticeEvent("Promo Success");
        } else if (hypeScore + opinionScore >= 28) {
            ev = GameEvent.LoadNoticeEvent("Promo Major Success");
        }

        ev.description = ev.description.Replace("<NUM_PEOPLE>", string.Format("{0:n0}", numPeople));
        GameEvent.Trigger(ev);

        // Apply the results to the company.
        OpinionEvent oe = new OpinionEvent(opinionScore, hypeScore);
        oe.name = promo.name;
        GameManager.Instance.playerCompany.ApplyOpinionEvent(oe);

        // Cleanup.
        Destroy(gameObject);
    }
}
