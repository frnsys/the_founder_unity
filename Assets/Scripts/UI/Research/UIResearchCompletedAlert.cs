using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIResearchCompletedAlert: UIEffectAlert {
    public UILabel nameLabel;
    public UITexture icon;

    private Technology technology_;
    public Technology technology {
        get { return technology_; }
        set {
            technology_ = value;
            nameLabel.text = technology_.name;
            bodyLabel.text = technology_.description;
            icon.mainTexture = technology_.icon;
            Extend(bodyLabel.height);

            RenderEffects(technology.effects);
            AdjustEffectsHeight();
        }
    }

    public GameObject manageResearchPrefab;
    public void SelectNewResearch() {
        UIManager uim = UIManager.Instance;

        // This is a hacky way of checking if the current popup
        // is already the research manager popup.
        if (uim.currentPopup != null) {
            // If it is already the research manager, just close this aler.
            if (string.Format("{0}(Clone)", uim.currentPopup.name) == manageResearchPrefab.name) {
                Close_();
                return;

            // Otherwise, close the current popup.
            } else {
                uim.ClosePopup();
            }
        }
        UIManager.Instance.OpenPopup(manageResearchPrefab);
        Close_();
    }
}


