/*
 * GameEvent Notification
 * ===================
 *
 * This manages the UI element
 * which is displayed when a gameEvent occurs.
 *
 */

using UnityEngine;
using System.Collections;

public class UIGameEventNotification: UIEffectAlert {
    public UIWidget title;
    public UILabel titleLabel;
    public UIGrid actionGrid;
    public UITexture image;

    public GameObject actionPrefab;

    private GameEvent gameEvent_;
    public GameEvent gameEvent {
        get { return gameEvent_; }
        set {
            gameEvent_ = value;
            titleLabel.text = gameEvent_.name;
            bodyLabel.text = gameEvent_.description;
            Extend(bodyLabel.height);

            RenderActions();
            RenderEffects(gameEvent_.effects);
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

    private void Extend(int amount) {
        int current = body.bottomAnchor.absolute;
        body.bottomAnchor.Set(title.transform, 0, current - amount);

        // Adjust height of the popup shadow.
        float actionHeight = (actionGrid.GetChildList().Count * actionGrid.cellHeight) - (actionGrid.cellHeight/2) + 3;
        UIWidget shadow = transform.Find("Shadow").GetComponent<UIWidget>();
        shadow.bottomAnchor.Set(body.transform, 0, -actionHeight);
    }

}


