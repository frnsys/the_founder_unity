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

            RenderActions();
            RenderEffects();
        }
    }

    private void RenderEffects() {
        // Clear out existing effect elements.
        while (effectGrid.transform.childCount > 0) {
            GameObject go = effectGrid.transform.GetChild(0).gameObject;
            NGUITools.DestroyImmediate(go);
        }

        // Render the unlock effects for this event.
        // Note that event unlocks are *not* rendered because
        // those are "hidden" effects.
        foreach (Industry i in gameEvent_.unlocks.industries) {
            RenderUnlockEffect("the " + i.name + " industry");
        }
        foreach (ProductType i in gameEvent_.unlocks.productTypes) {
            RenderUnlockEffect(i.name + " products");
        }
        foreach (Market i in gameEvent_.unlocks.markets) {
            RenderUnlockEffect("the " + i.name + " market");
        }
        foreach (Worker i in gameEvent_.unlocks.workers) {
            RenderUnlockEffect(i.name);
        }
        foreach (Item i in gameEvent_.unlocks.items) {
            RenderUnlockEffect(i.name);
        }
    }

    private void RenderUnlockEffect(string name) {
        GameObject unlockObj = NGUITools.AddChild(effectGrid.gameObject, unlockEffectPrefab);
        unlockObj.GetComponent<UIUnlockEffect>().Set(name);
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

    void OnEnable() {
        GameManager.Instance.Pause();
    }

    public void Close(GameObject actionObj) {
        NGUITools.DestroyImmediate(gameObject);

        GameManager.Instance.Resume();
    }

    public UILabel titleLabel;
    public UILabel descLabel;
    public UIGrid actionGrid;
    public UIGrid effectGrid;

    public GameObject actionPrefab;
    public GameObject unlockEffectPrefab;
}


