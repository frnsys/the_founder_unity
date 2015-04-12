using UnityEngine;
using System;
using System.Collections;

public class UIUpgradePerk : UIEffectAlert {
    private APerk _perk;
    public APerk perk {
        get { return _perk; }
        set {
            _perk = value;

            nameLabel.text = _perk.next.name;
            descLabel.text = _perk.next.description;
            totalLabel.text = string.Format("Upgrade for ${0:n}", _perk.next.cost);

            perkObj.GetComponent<MeshFilter>().mesh = _perk.next.mesh;

            RenderEffects(_perk.next.effects);
            AdjustEffectsHeight();
        }
    }

    public UILabel totalLabel;
    public UILabel nameLabel;
    public UILabel descLabel;
    public GameObject perkObj;

    void Update() {
        UIAnimator.Rotate(perkObj);
    }

    public Action callback;
    public void Upgrade() {
        bool upgraded = GameManager.Instance.playerCompany.UpgradePerk(perk);

        if (!upgraded)
            UIManager.Instance.Alert("Not enough cash.");

        if (callback != null)
            callback();

        Close_();
    }
}
