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

            if (worker_.robot) {
                bioLabel.text = "Description";
                statLabel.text = "Specifications";
                credit.gameObject.SetActive(false);
            }

            creativity.text = "Creativity: " + worker_.creativity;
            charisma.text = "Charisma: " + worker_.charisma;
            cleverness.text = "Cleverness: " + worker_.cleverness;
            productivity.text = "Productivity: " + worker_.productivity;
            happiness.text = "Happiness: " + worker_.happiness;
        }
    }

    public void SetBasicWorker(Worker w) {
        SetFuzzyWorker(w);
        quantObj.SetActive(false);
    }

    // Imprecise worker info.
    public void SetFuzzyWorker(Worker w) {
        worker = w;
        creativity.text = "Creativity: " + FuzzyStat(worker_.creativity.value).ToString();
        charisma.text = "Charisma: " + FuzzyStat(worker_.charisma.value).ToString();
        cleverness.text = "Cleverness: " + FuzzyStat(worker_.cleverness.value).ToString();
        productivity.text = "Productivity: " + FuzzyStat(worker_.productivity.value).ToString();
        happiness.text = "Happiness: " + FuzzyStat(worker_.happiness.value).ToString();
        credit.text += ". Disclaimer: This is a beta, expect some margin of error.";
        quantObj.SetActive(true);
    }

    public void SetQuantWorker(Worker w) {
        worker = w;
        quantObj.SetActive(true);
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
    public UILabel credit;
    public UIButton button;

    // These change for robots.
    public UILabel bioLabel;
    public UILabel statLabel;

    public GameObject quantObj;
}


