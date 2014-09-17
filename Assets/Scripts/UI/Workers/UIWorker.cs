/*
 * Worker Item
 * ===================
 *
 * Very simple item for Workers.
 *
 */

using UnityEngine;
using System.Collections;

public class UIWorker: MonoBehaviour {
    private Worker worker_;
    public Worker worker {
        get { return worker_; }
        set {
            worker_ = value;
            name.text = worker_.name;
            bio.text = worker_.bio;
            creativity.text = "Creativity: " + worker_.creativity;
            charisma.text = "Charisma: " + worker_.charisma;
            cleverness.text = "Cleverness: " + worker_.cleverness;
        }
    }

    public UILabel name;
    public UILabel bio;
    public UILabel creativity;
    public UILabel charisma;
    public UILabel cleverness;
}


