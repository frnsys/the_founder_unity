using UnityEngine;
using System;
using System.Collections;

public class MarketAnalysis : UIAlert {
    public UIGrid grid;

    public UILabel hype;
    public UILabel opinion;
    public UILabel reach;
    public UILabel share;
    public UIGrid starsGrid;
    public GameObject techPenalty;

    public void Setup(Product p) {
        Company c = GameManager.Instance.playerCompany;

        hype.text = string.Format("x{0:0.#}", 1 + c.publicity.value);
        opinion.text = string.Format("x{0:0.#}", 1 + c.opinion.value/100f);
        reach.text = string.Format("x{0:0.##}", Math.Round(c.marketSharePercent, 2));
        share.text = string.Format("{0:0.##}%", Math.Round(p.marketShare, 2));

        // Reset stars
        foreach (Transform t in starsGrid.transform) {
            t.gameObject.SetActive(false);
        }

        float stars = Mathf.Min(Mathf.Round(p.score * 5 * 2)/2f, 6f);

        for (int i=0; i < stars; i++) {
            starsGrid.transform.GetChild(i).gameObject.SetActive(true);
        }
        if (stars % 1 != 0) {
            starsGrid.transform.GetChild(starsGrid.transform.childCount - 1).gameObject.SetActive(true);
        }
        starsGrid.Reposition();

        techPenalty.SetActive(p.techPenalty);
        grid.Reposition();
    }
}

