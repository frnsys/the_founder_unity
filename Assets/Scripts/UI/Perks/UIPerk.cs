using UnityEngine;
using System.Collections;

public class UIPerk : UIEffectItem {
    private Perk _perk;
    public Perk perk {
        get { return _perk; }
        set {
            _perk = value.Clone();

            // Check if the perk is already owned.
            Perk ownedPerk = Perk.Find(_perk, company.perks);
            if (ownedPerk == null) {
                SetupUnownedPerk();
            } else {
                SetupOwnedPerk();
            }

            DisplayPerk();
        }
    }

    private Company company;
    public GameObject upgradePerkPrefab;

    public UILabel nameLabel;
    public UILabel descLabel;
    public UILabel costLabel;
    public UIButton button;
    public UILabel buttonLabel;
    public GameObject perkObj;

    private UIEventListener.VoidDelegate action;
    public void DoAction() {
        action(null);
    }

    void DisplayPerk() {
        nameLabel.text = _perk.current.name;
        descLabel.text = _perk.description;
        perkObj.GetComponent<MeshFilter>().mesh = _perk.mesh;

        RenderEffects(_perk.effects);
        AdjustEffectsHeight();
    }

    void SetupUnownedPerk() {
        costLabel.gameObject.SetActive(true);
        costLabel.text = string.Format("{0:C0}", perk.cost);
        buttonLabel.text = "Buy";
        action = DecideBuy;
    }

    void SetupOwnedPerk() {
        costLabel.gameObject.SetActive(false);
        buttonLabel.text = "Upgrade";
        action = DecideUpgrade;

        // Disable if there are no more upgrades available.
        if (!_perk.hasNext) {
            button.isEnabled = false;
            buttonLabel.text = "Maxed Out";
        } else if (!_perk.NextAvailable(company)) {
            button.isEnabled = false;
        } else {
            button.isEnabled = true;
        }
    }

    void Awake() {
        company = GameManager.Instance.playerCompany;
        UIEventListener.Get(button.gameObject).onClick += action;
    }

    void Update() {
        // Rotate the product, fancy.
        perkObj.transform.Rotate(0,0,-50*Time.deltaTime);
    }

    public void DecideUpgrade(GameObject obj) {
        // This is not very elegant but this is the only way we're using it.
        GameObject target = transform.parent.parent.parent.gameObject;

        GameObject upgradePerkPopup = NGUITools.AddChild(target, upgradePerkPrefab);
        UIUpgradePerk up =  upgradePerkPopup.GetComponent<UIUpgradePerk>();
        up.perk = perk;
        up.callback = UpgradedPerk;
    }

    void UpgradedPerk() {
        _perk = Perk.Find(_perk, company.perks);
        SetupOwnedPerk();
        DisplayPerk();
    }

    public void DecideBuy(GameObject obj) {
        UIManager.Instance.Confirm("This perk will cost you " + costLabel.text, delegate() {
                if (company.BuyPerk(_perk)) {
                    DisplayPerk();
                    SetupOwnedPerk();
                } else {
                    UIManager.Instance.Alert("You can't afford this perk.");
                }
        }, null);
    }
}
