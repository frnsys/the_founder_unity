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

            if (worker_.texture != null)
                employee.material.mainTexture = worker_.texture;

            if (worker_.description != "")
                bio.text = string.Format("{0} {1}", worker_.description, worker_.bio);
            else
                bio.text = worker_.bio;

            Company employer = GameManager.Instance.workerManager.EmployerForWorker(worker_);
            if (employer != null)
                title.text = string.Format("{0} at {1}", worker_.title, employer.name);
            else
                title.text = worker_.title;

            if (worker_.robot) {
                bioLabel.text = "Description";
                statLabel.text = "Specifications";
                credit.gameObject.SetActive(false);
            }

            creativity.text = string.Format("Creativity: {0}", worker_.creativity);
            charisma.text = string.Format("Charisma: {0}", worker_.charisma);
            cleverness.text = string.Format("Cleverness: {0}", worker_.cleverness);
            productivity.text = string.Format("Productivity: {0}", worker_.productivity);
            happiness.text = string.Format("Happiness: {0}", worker_.happiness);
        }
    }

    public void SetBasicWorker(Worker w) {
        SetFuzzyWorker(w);
        quantObj.SetActive(false);
    }

    // Imprecise worker info.
    public void SetFuzzyWorker(Worker w) {
        worker = w;
        creativity.text = string.Format("Creativity: {0}", FuzzyStat(worker_.creativity));
        charisma.text = string.Format("Charisma: {0}", FuzzyStat(worker_.charisma));
        cleverness.text = string.Format("Cleverness: {0}", FuzzyStat(worker_.cleverness));
        productivity.text = string.Format("Productivity: {0}", FuzzyStat(worker_.productivity));
        happiness.text = string.Format("Happiness: {0}", FuzzyStat(worker_.happiness));
        credit.text += ". Disclaimer: This is a beta, expect some margin of error.";
        quantObj.SetActive(true);
    }

    public void SetQuantWorker(Worker w) {
        worker = w;
        quantObj.SetActive(true);
    }

    private int FuzzyStat(Stat stat) {
        double fuzzy = (0.6 + Random.value) * stat.value;
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
    public MeshRenderer employee;

    // These change for robots.
    public UILabel bioLabel;
    public UILabel statLabel;

    public GameObject quantObj;
}


