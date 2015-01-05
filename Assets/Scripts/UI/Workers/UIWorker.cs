/*
 * Worker Item
 * ===================
 *
 * Very simple item for Workers.
 *
 */

using UnityEngine;
using System.Collections;

public class UIWorker : MonoBehaviour {
    private Worker worker_;
    public Worker worker {
        get { return worker_; }
        set {
            worker_ = value;
            name.text = worker_.name;
            bio.text = worker_.description + " " + worker_.bio;

            Company employer = GameManager.Instance.workerManager.EmployerForWorker(worker_);
            if (employer != null)
                title.text = worker_.title + " at " + employer.name;
            else
                title.text = worker_.title;

            creativity.text = "Creativity: " + worker_.creativity;
            charisma.text = "Charisma: " + worker_.charisma;
            cleverness.text = "Cleverness: " + worker_.cleverness;
            productivity.text = "Productivity: " + worker_.productivity;
            happiness.text = "Happiness: " + worker_.happiness;
        }
    }

    // Imprecise worker info.
    public void SetFuzzyWorker(Worker w) {
        worker = w;
        creativity.text = "Creativity: " + FuzzyStat(worker_.creativity.value).ToString();
        charisma.text = "Charisma: " + FuzzyStat(worker_.charisma.value).ToString();
        cleverness.text = "Cleverness: " + FuzzyStat(worker_.cleverness.value).ToString();
        productivity.text = "Productivity: " + FuzzyStat(worker_.productivity.value).ToString();
        happiness.text = "Happiness: " + FuzzyStat(worker_.happiness.value).ToString();
    }

    private int FuzzyStat(float value) {
        double fuzzy = (0.6 + Random.value) * value;
        return (int)fuzzy;
    }

    public UILabel name;
    public UILabel bio;
    public UILabel title;
    public UILabel creativity;
    public UILabel charisma;
    public UILabel cleverness;
    public UILabel productivity;
    public UILabel happiness;
}


