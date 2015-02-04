using UnityEngine;
using System.Collections;

public class UIGameEventNotification: UIEffectAlert {
    public UIWidget title;
    public UILabel titleLabel;
    public UILabel fromLabel;
    public UILabel toLabel;
    public UIGrid actionGrid;
    public UITexture image;

    public GameObject actionPrefab;

    private GameEvent gameEvent_;
    public GameEvent gameEvent {
        get { return gameEvent_; }
        set {
            gameEvent_ = value;

            Company c = GameManager.Instance.playerCompany;
            string companyName = c.name;

            titleLabel.text = gameEvent_.name;
            bodyLabel.text = gameEvent_.description.Replace("<PLAYERCOMPANY>", companyName);
            fromLabel.text = gameEvent_.from.Replace("<PLAYERCOMPANY>", companyName);

            if (toLabel != null)
                toLabel.text = "founder@" + companyName + ".com";

            if (gameEvent_.image != null)
                image.mainTexture = gameEvent_.image;

            Extend(bodyLabel.height);

            RenderActions();
            RenderEffects(gameEvent_.effects);

            // -1 because by default there is space for about 1 effect.
            Extend((int)((effectGrid.GetChildList().Count - 1) * effectGrid.cellHeight));
        }
    }

    private void RenderActions() {
        // Clear out existing action elements.
        while (actionGrid.transform.childCount > 0) {
            GameObject go = actionGrid.transform.GetChild(0).gameObject;
            UIEventListener.Get(go).onClick -= Close;
            NGUITools.DestroyImmediate(go);
        }

        // Render the available actions for this event.
        if (gameEvent_.actions.Count > 0) {
            foreach (EventAction action in gameEvent_.actions) {
                GameObject actionObj = NGUITools.AddChild(actionGrid.gameObject, actionPrefab);
                actionObj.GetComponent<UIEventActionButton>().action = action;

                // When this action button is clicked, close this notification.
                UIEventListener.Get(actionObj).onClick += Close;
            }
        } else {
            // Create a default "OK" action button.
            GameObject actionObj = NGUITools.AddChild(actionGrid.gameObject, actionPrefab);

            // When this action button is clicked, close this notification.
            UIEventListener.Get(actionObj).onClick += Close;
        }
    }

    public void Extend(int amount) {
        int current = body.bottomAnchor.absolute;
        body.bottomAnchor.Set(title.transform, 0, current - amount);

        // Adjust height of the popup shadow.
        float actionHeight = (actionGrid.GetChildList().Count * actionGrid.cellHeight) - (actionGrid.cellHeight/2) + 3;
        UIWidget shadow = transform.Find("Shadow").GetComponent<UIWidget>();
        shadow.bottomAnchor.Set(body.transform, 0, -actionHeight);
    }

}


