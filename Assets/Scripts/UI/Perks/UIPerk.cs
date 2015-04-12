using UnityEngine;
using System.Collections;

public class UIPerk : UIEffectItem {
    private static Color upgradeColor = new Color(1f, 0.9f, 0.53f);

    private APerk _perk;
    public APerk perk {
        get { return _perk; }
        set {
            _perk = value;

            // Check if the perk is already owned.
            APerk ownedPerk = company.OwnedPerk(_perk);
            if (ownedPerk == null) {
                _perk = ownedPerk;
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
        button.defaultColor = upgradeColor;
        button.hover = upgradeColor;
        button.pressed = upgradeColor;
        action = DecideUpgrade;

        // Disable if there are no more upgrades available.
        if (!_perk.hasNext) {
            button.isEnabled = false;
            buttonLabel.text = "Maxed Out";
        } else if (!_perk.NextAvailable(company)) {
            button.isEnabled = false;
            buttonLabel.text = "Upgrade Locked";
        } else {
            button.isEnabled = true;
        }
    }

    void Awake() {
        company = GameManager.Instance.playerCompany;
        UIEventListener.Get(button.gameObject).onClick += action;
    }

    void Update() {
        UIAnimator.Rotate(perkObj);
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
        _perk = company.OwnedPerk(_perk);
        SetupOwnedPerk();
        DisplayPerk();
    }

    public void DecideBuy(GameObject obj) {
        UIManager.Instance.Confirm("This perk will cost you " + costLabel.text, delegate() {
                if (company.BuyPerk(_perk)) {
                    Debug.Log("Perk bought");
                    DisplayPerk();
                    SetupOwnedPerk();
                } else {
                    UIManager.Instance.Alert("You can't afford this perk.");
                }
        }, null);
    }
}
