using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIResearchCompletedAlert: UIEffectAlert {
    public UILabel nameLabel;
    public GameObject discoveryPrefab;
    public GameObject selectedDiscoveryItem;

    // For mapping GameObjects (by id) to Discoveries.
    public Dictionary<int, Discovery> discoveries = new Dictionary<int, Discovery>();

    private Discovery discovery_;
    public Discovery discovery {
        get { return discovery_; }
        set {
            discovery_ = value;
            nameLabel.text = discovery_.name;
            bodyLabel.text = discovery_.description;
            Extend(bodyLabel.height);

            RenderEffects(discovery.effects);

            // -1 because by default there is space for about 1 effect.
            Extend((int)((effectGrid.GetChildList().Count - 1) * effectGrid.cellHeight));
        }
    }

    public void ShowNewResearchOptions() {
        transform.Find("Notification Window").gameObject.SetActive(false);
        transform.Find("New Research Window").gameObject.SetActive(true);
    }

    void OnEnable() {
        Show();

        // TO DO needs to load the next three discoveries to choose from.
        //foreach (Discovery d in GameManager.Instance.researchManager.nextDiscoveries) {
        //}
        // Temporary:
        Discovery di = Resources.Load("Discoveries/Electricity") as Discovery;

        GameObject discoveriesGrid = transform.Find("New Research Window/Body/Discoveries").gameObject;
        int width = discoveriesGrid.GetComponent<UIWidget>().width;

        List<Discovery> nextDiscoveries = new List<Discovery>();
        nextDiscoveries.Add(di);
        nextDiscoveries.Add(di);
        nextDiscoveries.Add(di);

        foreach (Discovery d in nextDiscoveries) {
            GameObject dItem = NGUITools.AddChild(discoveriesGrid, discoveryPrefab);
            dItem.transform.Find("Discovery Name").GetComponent<UILabel>().text = d.name;

            // Have to manually set all of this.
            int height = dItem.transform.GetComponent<UITexture>().height;
            dItem.transform.GetComponent<UITexture>().width = width;
            dItem.transform.GetComponent<UITexture>().depth = 10;
            dItem.transform.GetComponent<BoxCollider>().size = new Vector3(width, height, 1);

            UIEventListener.Get(dItem).onClick += SelectDiscovery;
            discoveries.Add(dItem.GetInstanceID(), d);
        }
        SelectDiscovery( discoveriesGrid.transform.GetChild(0).gameObject );

        discoveriesGrid.GetComponent<UICenteredGrid>().Reposition();
    }

    void SelectDiscovery(GameObject obj) {
        // Revert previously selected item.
        if (selectedDiscoveryItem != null) {
            selectedDiscoveryItem.GetComponent<UIButton>().defaultColor = new Color(1f, 1f, 1f, 1f);
            selectedDiscoveryItem.transform.Find("Discovery Name").gameObject.GetComponent<UILabel>().color = new Color(0.1f, 0.1f, 0.1f, 1f);
        }

        // Highlight selected item.
        selectedDiscoveryItem = obj;
        obj.GetComponent<UIButton>().defaultColor = new Color(0.57f, 0.41f, 0.98f, 1f);
        obj.transform.Find("Discovery Name").gameObject.GetComponent<UILabel>().color = new Color(1f, 1f, 1f, 1f);
    }

    public void ConfirmDiscovery() {
        Discovery d = discoveries[selectedDiscoveryItem.GetInstanceID()];
        GameManager.Instance.researchManager.BeginResearch(d);
        Close_();
    }
}


