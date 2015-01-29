using UnityEngine;
using System.Collections;

public class UIUpgradePerk : UIEffectAlert {
    private Perk _perk;
    public Perk perk {
        get { return _perk; }
        set {
            _perk = value;

            nameLabel.text = _perk.next.name;
            descLabel.text = _perk.next.description;
            totalLabel.text = string.Format("${0:n}", _perk.next.cost);

            perkObj.GetComponent<MeshFilter>().mesh = _perk.next.mesh;
            perkObj.GetComponent<MeshRenderer>().material.mainTexture = _perk.next.texture;

            RenderEffects(_perk.next.effects);
            AdjustEffectsHeight();
        }
    }

    public UILabel totalLabel;
    public UILabel nameLabel;
    public UILabel descLabel;
    public GameObject perkObj;

    void Update() {
        // Rotate the product, fancy.
        float rotation = perkObj.transform.rotation.z;
        perkObj.transform.Rotate(0,0,rotation - 1.5f);
    }

    public void Upgrade() {
        GameManager.Instance.playerCompany.UpgradePerk(perk);
        Close_();
    }
}
