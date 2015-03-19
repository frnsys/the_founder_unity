using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UILobby : UIEffectItem {
    private Lobby _lobby;
    public Lobby lobby {
        get { return _lobby; }
        set {
            _lobby = value;

            if (company.lobbies.Contains(_lobby)) {
                SetupOwned();
            } else {
                SetupUnowned();
            }

            DisplayProject();
        }
    }

    private Company company;

    public UILabel nameLabel;
    public UILabel descLabel;
    public UIButton button;
    public UILabel buttonLabel;

    void DisplayProject() {
        nameLabel.text = _lobby.name;
        descLabel.text = _lobby.description;

        RenderEffects(_lobby.effects);
        AdjustEffectsHeight();
    }

    void SetupUnowned() {
        buttonLabel.text = string.Format("{0:C0}", _lobby.cost);
    }

    void SetupOwned() {
        button.isEnabled = false;
        buttonLabel.text = "In Effect";
    }

    void Awake() {
        company = GameManager.Instance.playerCompany;
        UIEventListener.Get(button.gameObject).onClick += BuyLaw;
    }

    void Update() {
        UpdateEffectWidths();
    }

    public void BuyLaw(GameObject obj) {
        UIManager.Instance.Confirm(string.Format("Lobbying for this law will cost {0:C0}.", _lobby.cost), delegate() {
            if (!company.BuyLobby(_lobby)) {
                UIManager.Instance.Alert("You can't afford to do this lobbying.");
            }
        }, null);
    }
}
