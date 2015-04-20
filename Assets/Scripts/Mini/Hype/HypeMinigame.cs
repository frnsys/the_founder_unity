using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HypeMinigame : MonoBehaviour {
    public int pucks = 3;
    public HypePuck puck;
    public UILabel pitchesLabel;
    public UILabel scoreLabel;
    public Transform gameBoard;
    public Promo promo;
    public Texture[] blebTextures;
    public GameObject hypeTargetPrefab;
    public Transform[] tiers;

    private float hypeScore = 0;
    private List<HypeTarget> targets;

    public GameObject level;
    public void Setup(Promo promo) {
        // This is a hack so that the mentor message
        // doesn't have to deal with layering issues with the blebs.
        level.SetActive(false);
        Invoke("StartGame", 0.5f);

        targets = new List<HypeTarget>();
        for (int i=0; i < (int)(promo.cost/2000); i++) {
            GameObject hypeTarget = Instantiate(hypeTargetPrefab) as GameObject;
            HypeTarget ht = hypeTarget.GetComponent<HypeTarget>();

            HypeTarget.Type t = HypeTarget.RandomType;
            ht.transform.parent = tiers[(int)t];
            ht.Setup(this, t);

            targets.Add(ht);
        }
    }

    void StartGame() {
        level.SetActive(true);
    }

    void OnEnable() {
        EventTimer.Pause();
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
        HypePuck.Fired -= Fired;
        EventTimer.Resume();
    }

    void Scored(float hypePoints) {
        hypeScore += hypePoints;
        UpdateLabels();
    }

    void Fired() {
        pucks--;
        UpdateLabels();

        // Reset after timeout.
        StartCoroutine("Timeout");
    }

    IEnumerator Timeout() {
        yield return new WaitForSeconds(3f);
        Completed();
    }

    void Completed() {
        StopCoroutine("Timeout");
        if (pucks >= 0) {
            foreach (HypeTarget ht in targets) {
                ht.Reset();
            }
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
        scoreLabel.text = string.Format("Hype: [c][56FB92]{0:0.#}[-][/c]", hypeScore);
    }

    void EndGame() {
        int numPeople = (int)(Mathf.Pow(hypeScore, 2) * 100 * (Random.value + 0.75f));

        AGameEvent ev = GameEvent.LoadNoticeEvent("Promo Failure");
        if (hypeScore * 1000 >= promo.cost * 0.7) {
            ev = GameEvent.LoadNoticeEvent("Promo Meh");
        } else if (hypeScore * 1000 >= promo.cost) {
            ev = GameEvent.LoadNoticeEvent("Promo Success");
        } else if (hypeScore * 1000 >= promo.cost * 1.5) {
            ev = GameEvent.LoadNoticeEvent("Promo Major Success");
        }

        string desc = ev.gameEvent.description;
        ev.gameEvent.description = ev.gameEvent.description.Replace("<NUM_PEOPLE>", string.Format("{0:n0}", numPeople));
        GameEvent.Trigger(ev.gameEvent);
        ev.gameEvent.description = desc;

        // Apply the results to the company.
        OpinionEvent oe = new OpinionEvent(0, hypeScore);
        oe.name = promo.name;
        GameManager.Instance.playerCompany.ApplyOpinionEvent(oe);

        // Cleanup.
        Destroy(gameObject);

        if (Done != null)
            Done();
    }

    public void Quit() {
        UIManager.Instance.Confirm("Are you sure you want to prematurely end your promotion?", delegate() {
            EndGame();
        }, null);
    }

    static public event System.Action Done;
}
