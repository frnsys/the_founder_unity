using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIManageInfrastructure : MonoBehaviour {
    public GameObject infrastructureItemPrefab;
    private UICenteredGrid grid;
    private Company playerCompany;
    private List<GameObject> gridItems;

    void Awake() {
        gridItems = new List<GameObject>();
        grid = gameObject.GetComponent<UICenteredGrid>();
        playerCompany = GameManager.Instance.playerCompany;

        // Create an item for each infrastructure type.
        foreach (Infrastructure.Type t in Infrastructure.Types) {
            GameObject item = NGUITools.AddChild(gameObject, infrastructureItemPrefab);
            item.transform.Find("Label").GetComponent<UILabel>().text = t.ToString();

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
            GameObject buyButton = item.transform.Find("Buy Button").gameObject;
            UIEventListener.Get(buyButton).onClick += buyInf;

            GameObject desButton = item.transform.Find("Destroy Button").gameObject;
            UIEventListener.Get(desButton).onClick += desInf;

            gridItems.Add(item);
        }
        UpdateButtons();

        grid.Reposition();
    }

    void Update() {
        Infrastructure capacity = playerCompany.infrastructureCapacity;
        Infrastructure infra    = playerCompany.infrastructure;

        // Update the amount of infrastructure against the total capacity.
        for (int i=0; i<gridItems.Count; i++) {
            GameObject item = gridItems[i];
            Infrastructure.Type t = Infrastructure.Types[i];
            item.transform.Find("Used and Capacity").GetComponent<UILabel>().text = infra[t].ToString() + "/" +  capacity[t].ToString();
        }
    }

    void UpdateButtons() {
        Infrastructure capacity = playerCompany.infrastructureCapacity;
        Infrastructure infra    = playerCompany.infrastructure;

        // Update the buy/destroy buttons for infrastructure (disable/enable them).
        for (int i=0; i<gridItems.Count; i++) {
            GameObject item = gridItems[i];
            Infrastructure.Type t = Infrastructure.Types[i];

            GameObject buyButton = item.transform.Find("Buy Button").gameObject;
            GameObject desButton = item.transform.Find("Destroy Button").gameObject;

            Infrastructure inf = Infrastructure.ForType(t);
            buyButton.GetComponent<UIButton>().isEnabled = playerCompany.HasCapacityFor(inf);
            desButton.GetComponent<UIButton>().isEnabled = !(infra[t] == 0);
        }
    }
}


