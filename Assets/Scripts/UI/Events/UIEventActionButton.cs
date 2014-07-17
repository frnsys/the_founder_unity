/*
 * EventAction Button
 * ===================
 *
 * Shows up in GameEvent notifications
 * as an action.
 *
 */

using UnityEngine;
using System.Collections;

public class UIEventActionButton : MonoBehaviour {
    private EventAction action_;
    public EventAction action {
        get { return action_; }
        set {
            action_ = value;
            titleLabel.text = action_.name;
        }
    }

    public UILabel titleLabel;

    void OnClick() {
        if (action_ != null) {
            EventAction.Trigger(action_);
        }
    }
}


