using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInfrastructure : MonoBehaviour {
    public List<UIInfrastructureItem> infItems;
    public UIGrid grid;
    private Company playerCompany;
    private Infrastructure capacity;

    void Awake() {
        playerCompany = GameManager.Instance.playerCompany;
        capacity = playerCompany.infrastructureCapacity;
        ShowInfrastructure();
    }

    private void ShowInfrastructure() {
        foreach (UIInfrastructureItem infItem in infItems) {
            Infrastructure.Type t = infItem.type;
            infItem.costLabel.text = string.Format("{0:C0}/mo each", Infrastructure.baseCost * (GameManager.Instance.infrastructureCostMultiplier[t]/100f));

            // Setup button actions.
            Infrastructure.Type lT = t;
            UIEventListener.VoidDelegate buyInf = delegate(GameObject obj) {
                Infrastructure inf = Infrastructure.ForType(lT);
                if (!playerCompany.BuyInfrastructure(inf)) {
                    UIManager.Instance.Alert("You don't have enough capital for a new piece of infrastructure.");
                };
                UpdateButtons();
            };
            UIEventListener.VoidDelegate desInf = delegate(GameObject obj) {
                Infrastructure inf = Infrastructure.ForType(lT);
                playerCompany.DestroyInfrastructure(inf);
                UpdateButtons();
            };

            // Bind actions to buttons.
            UIEventListener.Get(infItem.buyButton.gameObject).onClick += buyInf;
            UIEventListener.Get(infItem.desButton.gameObject).onClick += desInf;
        }

        UpdateButtons();
        grid.Reposition();
    }

    private void UpdateButtons() {
        // Update the buy/destroy buttons for infrastructure (disable/enable them).
        foreach (UIInfrastructureItem infItem in infItems) {
            Infrastructure.Type t = infItem.type;
            Infrastructure inf = Infrastructure.ForType(t);
            infItem.buyButton.isEnabled = playerCompany.HasCapacityFor(inf);
            infItem.desButton.isEnabled = !(playerCompany.infrastructure[t] == 0);
        }

        // Update the amount of infrastructure against the total capacity.
        Infrastructure used = playerCompany.usedInfrastructure;
        Infrastructure infra = playerCompany.infrastructure;
        Infrastructure avail = capacity - infra;
        foreach (UIInfrastructureItem infItem in infItems) {
            Infrastructure.Type t = infItem.type;
            string plural = Infrastructure.Plural(t);

            string use = string.Format("Using [c][1685FA]{0}[-][/c] out of [c][6A53F7]{1}[-][/c] {2}.", used[t], infra[t], plural);
            if (used[t] == infra[t]) {
                use = string.Format("Using [c][EF4542]{0}[-][/c] out of [c][EF4542]{1}[-][/c] {2}.", used[t], infra[t], plural);
            }

            string cap = string.Format("Room to buy [c][56FB92]{0}[-][/c] more {1}.", avail[t], plural);
            if (avail[t] == 0) {
                cap = string.Format("Room to buy [c][EF4542]{0}[-][/c] more {1}.", avail[t], plural);
            }
            infItem.amountLabel.text = string.Format("{0}\n{1}", use, cap);
        }
    }
}


