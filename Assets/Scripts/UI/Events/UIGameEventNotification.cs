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

public class UIGameEventNotification: MonoBehaviour {
    private GameEvent gameEvent_;
    public GameEvent gameEvent {
        get { return gameEvent_; }
        set {
            gameEvent_ = value;
            titleLabel.text = gameEvent_.name;
            descLabel.text = gameEvent_.description;

            // Clear out existing action elements.
            while (actionGrid.transform.childCount > 0) {
                GameObject go = actionGrid.transform.GetChild(0).gameObject;
                UIEventListener.Get(go).onClick -= Close;
                NGUITools.DestroyImmediate(go);
            }

            if (gameEvent.actions.Count > 0) {
                foreach (EventAction action in gameEvent_.actions) {
                    GameObject actionObj = NGUITools.AddChild(actionGrid.gameObject, actionPrefab);
                    actionObj.GetComponent<UIEventActionButton>().action = action;
                    UIEventListener.Get(actionObj).onClick += Close;
                }
            } else {
                // Create a default "OK" action button.
                GameObject actionObj = NGUITools.AddChild(actionGrid.gameObject, actionPrefab);
                UIEventListener.Get(actionObj).onClick += Close;
            }
        }
    }

    void OnEnable() {
        // Pause
        Time.timeScale = 0;
    }

    public void Close(GameObject actionObj) {
        NGUITools.DestroyImmediate(gameObject);

        // Unpause
        Time.timeScale = 1;
    }

    public UILabel titleLabel;
    public UILabel descLabel;
    public UIGrid actionGrid;

    public GameObject actionPrefab;
}


