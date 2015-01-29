using UnityEngine;
using System.Collections;

public class UIPerk : MonoBehaviour {
    private Perk _perk;
    public Perk perk {
        get { return _perk; }
        set {
            _perk = value;
            nameLabel.text = _perk.name;
            descLabel.text = _perk.description;

            perkObj.GetComponent<MeshFilter>().mesh = _perk.mesh;
            perkObj.GetComponent<MeshRenderer>().material.mainTexture = _perk.texture;

            // Disable if there are no more upgrades available.
            if (!_perk.hasNext) {
                transform.Find("Upgrade Button").GetComponent<UIButton>().isEnabled = false;
                transform.Find("Upgrade Button/Label").GetComponent<UILabel>().text = "Maxed Out";
            } else if (!_perk.NextAvailable(company)) {
                transform.Find("Upgrade Button").GetComponent<UIButton>().isEnabled = false;
            } else {
                transform.Find("Upgrade Button").GetComponent<UIButton>().isEnabled = true;
            }
        }
    }

    private Company company;
    public GameObject upgradePerkPrefab;

    public UILabel nameLabel;
    public UILabel descLabel;
    public GameObject perkObj;

    void Start() {
        company = GameManager.Instance.playerCompany;
    }

    void Update() {
        // Rotate the product, fancy.
        perkObj.transform.Rotate(0,0,-50*Time.deltaTime);

        // Constantly update its status.
        perk = _perk;
    }

    public void DecideUpgrade() {
        // This is not very elegant but this is the only way we're using it.
        GameObject target = transform.parent.parent.gameObject;

        GameObject upgradePerkPopup = NGUITools.AddChild(target, upgradePerkPrefab);
        upgradePerkPopup.GetComponent<UIUpgradePerk>().perk = perk;
    }
}
