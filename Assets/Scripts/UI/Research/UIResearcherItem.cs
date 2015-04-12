using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIResearcherItem : MonoBehaviour {
    private AWorker worker_;
    public AWorker worker {
        get { return worker_; }
        set {
            worker_ = value;
            displayObject.GetComponent<MeshRenderer>().material = worker_.material;

            nameLabel.text = worker_.name;
            skillsLabel.text = string.Format(":DESIGN: {0}     :ENGINEERING: {1}     :MARKETING: {2}     :PRODUCTIVITY: {3}", worker_.creativity, worker_.cleverness, worker_.charisma, worker_.productivity);
        }
    }

    public GameObject displayObject;
    public UILabel nameLabel;
    public UILabel skillsLabel;
}


