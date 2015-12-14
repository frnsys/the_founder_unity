using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HypeMinigame : MonoBehaviour {
    private int pucks = 0;
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
        pucks = promo.pucks;
        UpdateLabels();

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

            // TODO make these based on public opinion or something
            if (Random.value <= 0.1) {
                ht.Outrage();
            } else if (Random.value <= 0.2) {
                ht.Happy();
            }


            targets.Add(ht);
        }
    }

    void StartGame() {
        level.SetActive(true);
    }

    void OnEnable() {
        EventTimer.Pause();
        HypeTarget.Scored += Scored;
        HypePuck.Fired += Fired;
        UpdateLabels();

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Puck"), LayerMask.NameToLayer("TargetPucks"), true);
    }

    void OnDisable() {
        HypeTarget.Scored -= Scored;
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
        yield return new WaitForSeconds(1.2f);

        // If puck is still visible, wait...
        while (puck.isVisible) {
            yield return null;
        }

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
        // Cleanup.
        Destroy(gameObject);

        if (Done != null)
            Done(hypeScore);
    }

    public void Quit() {
        UIManager.Instance.Confirm("Are you sure you want to prematurely end your promotion?", delegate() {
            EndGame();
        }, null);
    }

    static public event System.Action<float> Done;
}
