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
    private AWorker worker_;
    public AWorker worker {
        get { return worker_; }
        set {
            worker_ = value;
            name.text = worker_.name;

            if (worker_.material != null)
                employee.material = worker_.material;

            if (worker_.description != "")
                bio.text = string.Format("{0}\n- {1}", worker_.description, string.Join("\n- ", worker_.bio.ToArray()));
            else
                bio.text = string.Format("- {0}", string.Join("\n- ", worker_.bio.ToArray()));

            Company employer = GameManager.Instance.workerManager.EmployerForWorker(worker_);
            if (employer != null)
                title.text = string.Format("{0} at {1}", worker_.title, employer.name);
            else
                title.text = worker_.title;

            if (worker_.robot) {
                bioLabel.text = "Description";
                statLabel.text = "Specifications";
            }

            creativity.text = string.Format("Creativity: {0}", worker_.creativity);
            charisma.text = string.Format("Charisma: {0}", worker_.charisma);
            cleverness.text = string.Format("Cleverness: {0}", worker_.cleverness);
            productivity.text = string.Format("Productivity: {0}", worker_.productivity);
            happiness.text = string.Format("Happiness: {0}", worker_.happiness);
        }
    }

    public void SetBasicWorker(AWorker w) {
        worker = w;
        quantObj.SetActive(false);
    }

    public void SetQuantWorker(AWorker w) {
        worker = w;
        quantObj.SetActive(true);
    }

    public UILabel name;
    public UILabel bio;
    public UILabel title;
    public UILabel creativity;
    public UILabel charisma;
    public UILabel cleverness;
    public UILabel productivity;
    public UILabel happiness;
    public UILabel personalInfo;
    public UIButton button;
    public UIButton otherButton;
    public MeshRenderer employee;

    // These change for robots.
    public UILabel bioLabel;
    public UILabel statLabel;

    public GameObject quantObj;
}


