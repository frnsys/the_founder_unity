using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInfrastructure : MonoBehaviour {
    public UISimpleGrid grid;
    public UILabel infrastructureLabel;
    public GameObject infrastructureItemPrefab;
    private Company company;

    void Awake() {
        company = GameManager.Instance.playerCompany;
        ShowInfrastructure();
        SetupPrompt();
    }

    private void ShowInfrastructure() {
        // Create owned items.
        foreach (Infrastructure.Type t in Infrastructure.Types) {
            for (int i=0; i < company.usedInfrastructure[t]; i++) {
                GameObject infItem = NGUITools.AddChild(grid.gameObject, infrastructureItemPrefab);
                infItem.GetComponent<UIInfrastructureItem>().type = t;
                infItem.GetComponent<UIInfrastructureItem>().used = true;
                SetupButton(infItem);
            }
        }

        foreach (Infrastructure.Type t in Infrastructure.Types) {
            for (int i=0; i < company.availableInfrastructure[t]; i++) {
                GameObject infItem = NGUITools.AddChild(grid.gameObject, infrastructureItemPrefab);
                infItem.GetComponent<UIInfrastructureItem>().type = t;
                SetupButton(infItem);
            }
        }

        // Create empty spaces.
        for (int i=0; i < company.availableInfrastructureCapacity; i++) {
            GameObject infItem = NGUITools.AddChild(grid.gameObject, infrastructureItemPrefab);
            infItem.GetComponent<UIInfrastructureItem>().empty = true;
                SetupButton(infItem);
        }

        grid.Reposition();
    }

    private void SetupButton(GameObject button) {
        UIEventListener.VoidDelegate showPrompt = delegate(GameObject obj) {
            UIInfrastructureItem item = button.GetComponent<UIInfrastructureItem>();
            if (!item.used) {
                prompt.SetActive(true);
                currentItem = item;
            } else {
                UIManager.Instance.Alert("This infrastructure is being used by a product. Shutdown the product or wait for it to be discontinued to reclaim the infrastructure.");
            }
        };
        UIEventListener.Get(button).onClick += showPrompt;
    }

    private UIInfrastructureItem currentItem;
    public GameObject prompt;
    public UILabel[] costLabels;

    public void BuyInfrastructure(Infrastructure.Type t) {
        Infrastructure inf = Infrastructure.ForType(t);

        // If the this slot is currently occupied
        // and the company will be able to afford the new infrastructure,
        // destroy the existing one first.
        if (!currentItem.empty && company.cash.value >= inf.cost) {
            company.DestroyInfrastructure(Infrastructure.ForType(currentItem.type));
        }

        if (!company.BuyInfrastructure(inf)) {
            UIManager.Instance.Alert("You don't have enough capital for a new piece of that infrastructure.");
        } else {
            currentItem.type = t;
            prompt.SetActive(false);
        }
    }
    public void ClosePrompt() {
        currentItem = null;
        prompt.SetActive(false);
    }

    private void SetupPrompt() {
        foreach (Infrastructure.Type t in Infrastructure.Types) {
            costLabels[(int)t].text = string.Format("{0:C0}/mo", Infrastructure.baseCost * (GameManager.Instance.infrastructureCostMultiplier[t]/100f));
        }
    }

    void Update() {
        infrastructureLabel.text = company.infrastructure.ToStringWithEmpty();
    }
}


