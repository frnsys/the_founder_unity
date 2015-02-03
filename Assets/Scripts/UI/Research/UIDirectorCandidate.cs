using UnityEngine;
using System.Collections;

public class UIDirectorCandidate : MonoBehaviour {
    private Worker worker_;
    public Worker worker {
        get { return worker_; }
        set {
            worker_ = value;
            name.text = worker_.name;

            creativity.text = "Creativity: " + worker_.creativity;
            charisma.text = "Charisma: " + worker_.charisma;
            cleverness.text = "Cleverness: " + worker_.cleverness;
            productivity.text = "Productivity: " + worker_.productivity;
            happiness.text = "Happiness: " + worker_.happiness;
        }
    }

    public UILabel name;
    public UILabel creativity;
    public UILabel charisma;
    public UILabel cleverness;
    public UILabel productivity;
    public UILabel happiness;

    public void AppointAsResearchCzar() {
        UIManager.Instance.Confirm("Are you sure you want to appoint " + worker_.name + " as your Director of Research?", delegate() {
            GameManager.Instance.playerCompany.ResearchCzar = worker_;
        }, null);
    }

    public void AppointAsOpinionCzar() {
        UIManager.Instance.Confirm("Are you sure you want to appoint " + worker_.name + " as your Director of Communications?", delegate() {
            GameManager.Instance.playerCompany.OpinionCzar = worker_;
        }, null);
    }
}


