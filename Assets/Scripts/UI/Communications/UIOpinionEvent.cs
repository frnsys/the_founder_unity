using UnityEngine;
using System.Collections;

public class UIOpinionEvent : MonoBehaviour {
    private OpinionEvent opinionEvent_;
    public OpinionEvent opinionEvent {
        get { return opinionEvent_; }
        set {
            opinionEvent_ = value;
            label.text = opinionEvent_.name;
            opinion.text = opinionEvent_.opinion.value.ToString();
        }
    }

    public UILabel label;
    public UILabel opinion;
}


