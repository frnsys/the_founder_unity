using UnityEngine;

public class HypeMinigame : MonoBehaviour {
    public int pucks = 3;
    public HypePuck puck;
    public UILabel pitchesLabel;
    public UILabel scoreLabel;
    public Transform gameBoard;
    public Promo promo;
    public Texture[] blebTextures;

    private float hypeScore = 0;

    private GameObject level;
    public void Setup(Promo promo) {
        level = Instantiate(promo.level) as GameObject;
        level.transform.parent = gameBoard;

        foreach (HypeTarget ht in GetComponentsInChildren<HypeTarget>()) {
            ht.Setup(this);
        }

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
        HypePuck.Fired += Fired;
        UpdateLabels();
    }

    void OnDisable() {
        HypeTarget.Scored -= Scored;
        HypeTarget.Completed -= Completed;
        HypePuck.Completed -= Completed;
    }

    void Scored(float hypePoints) {
        hypeScore += hypePoints;
        UpdateLabels();
    }

    void Fired() {
        pucks--;
        UpdateLabels();
    }

    void Completed() {
        if (pucks >= 0) {
            puck.Reset();
        } else {
            // Game Over
            puck.gameObject.SetActive(false);
            EndGame();
        }
        UpdateLabels();
    }

    void UpdateLabels() {
        pitchesLabel.text = string.Format("Pitches: [c][EFD4F1]{0}[-][/c]", pucks + 1);
        scoreLabel.text = string.Format("Hype: [c][56FB92]{0}[-][/c]", hypeScore);
    }

    void EndGame() {
        int numPeople = (int)(Mathf.Pow(hypeScore, 2) * 1000 * (Random.value + 0.75f));

        GameEvent ev = GameEvent.LoadNoticeEvent("Promo Failure");
        if (hypeScore >= 8) {
            ev = GameEvent.LoadNoticeEvent("Promo Success");
        } else if (hypeScore >= 28) {
            ev = GameEvent.LoadNoticeEvent("Promo Major Success");
        }

        ev.description = ev.description.Replace("<NUM_PEOPLE>", string.Format("{0:n0}", numPeople));
        GameEvent.Trigger(ev);

        // Apply the results to the company.
        OpinionEvent oe = new OpinionEvent(0, hypeScore);
        oe.name = promo.name;
        GameManager.Instance.playerCompany.ApplyOpinionEvent(oe);

        // Cleanup.
        Destroy(gameObject);
    }
}
