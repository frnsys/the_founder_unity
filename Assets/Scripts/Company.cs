using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Company {
    public string name;
    public float cash = 1000;
    public int sizeLimit = 10;

    public List<Character> founders = new List<Character>();
    private List<GameObject> _workers = new List<GameObject>();
    public ReadOnlyCollection<GameObject> workers {
        get { return _workers.AsReadOnly(); }
    }

    public Company(string name_) {
        name = name_;
    }

    public bool HireWorker(GameObject worker) {
        if (_workers.Count < sizeLimit) {
            _workers.Add(worker);
            return true;
        }
        return false;
    }
    public void FireWorker(GameObject worker) {
        _workers.Remove(worker);
    }

    public void DevelopProduct(IProduct product) {
        float charisma = 0;
        float creativity = 0;
        float cleverness = 0;
        float progress = 0;

        foreach (GameObject worker in workers) {
            Worker w = worker.GetComponent<Worker>();
            charisma += w.charisma.value;
            creativity += w.creativity.value;
            cleverness += w.cleverness.value;
            progress += w.productivity.value;
        }

        product.Develop(progress, charisma, creativity, cleverness);
    }

    public void Pay() {
        foreach (GameObject worker in workers) {
            cash -= worker.GetComponent<Worker>().salary;
        }
    }
}


