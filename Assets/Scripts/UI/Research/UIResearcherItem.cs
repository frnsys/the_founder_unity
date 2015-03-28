using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIResearcherItem : MonoBehaviour {
    private Worker worker_;
    public Worker worker {
        get { return worker_; }
        set {
            worker_ = value;
            displayObject.GetComponent<MeshRenderer>().material = worker_.material;

            nameLabel.text = worker_.name;
            skillsLabel.text = string.Format(":DESIGN: {0}     :ENGINEERING: {1}     :MARKETING: {2}     :PRODUCTIVITY: {3}", worker_.creativity.value, worker_.cleverness.value, worker_.charisma.value, worker_.productivity.value);
        }
    }

    public GameObject displayObject;
    public UILabel nameLabel;
    public UILabel skillsLabel;
}


