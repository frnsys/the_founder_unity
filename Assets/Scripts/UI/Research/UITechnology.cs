using UnityEngine;
using System.Linq;
using System.Collections;

public class UITechnology : UIEffectItem {
    public static System.Action BoughtTech;

    private Technology technology_;
    public Technology technology {
        get { return technology_; }
        set {
            technology_ = value;
            label.text = technology_.name;
            description.text = technology_.description;
            image.mainTexture = technology_.icon;
            cost.text = string.Format("Costs {0:C0}", technology_.cost);

            UpdateLock();

            RenderEffects(technology_.effects);
            AdjustEffectsHeight();
        }
    }

    public bool researched {
        set {
            button.isEnabled = !value;
            if (value) {
                buttonLabel.text = "Researched";
            } else {
                buttonLabel.text = "Research";
            }
        }
    }

    public UILabel label;
    public UILabel cost;
    public UILabel description;
    public UILabel buttonLabel;
    public UIButton button;
    public UITexture image;

    public UIWidget info;
    public GameObject lockObj;

    public void SelectTech() {
        UIManager.Instance.Confirm("Are you sure you want to research this technology?", delegate() {
            if (!GameManager.Instance.playerCompany.BuyTechnology(technology_)) {
                UIManager.Instance.Alert("You don't have enough capital for this technology.");
            } else {
                researched = true;
            }
        }, null);
    }

    void OnEnable() {
        BoughtTech += UpdateLock;
    }

    void OnDisable() {
        BoughtTech -= UpdateLock;
    }

    void Update() {
        UpdateEffectWidths();
    }

    public void UpdateLock() {
        if (technology_.isAvailable(GameManager.Instance.playerCompany)) {
            Unlock();
        } else {
            Lock();
        }
    }

    void Lock() {
        image.alpha = 0.5f;
        info.alpha = 0.5f;
        lockObj.SetActive(true);
        button.gameObject.SetActive(false);

        label.text = "???";
        description.text = "";
    }

    void Unlock() {
        image.alpha = 1f;
        info.alpha = 1f;
        lockObj.SetActive(false);
        button.gameObject.SetActive(true);

        label.text = technology_.name;
        description.text = technology_.description;
    }

    public void ShowRequirements() {
        string requirements = string.Format("Requires the {0} vertical.", technology_.requiredVertical.name);
        if (technology_.requiredTechnologies.Count > 0) {
            requirements = string.Format("Requires the {0} vertical and {1}.", technology_.requiredVertical.name, string.Join(", ", technology_.requiredTechnologies.Select(t => t.name).ToArray()));
        }
        UIManager.Instance.Alert(requirements);
    }
}


