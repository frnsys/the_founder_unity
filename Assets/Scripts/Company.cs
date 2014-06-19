using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Company : MonoBehaviour {
    public float cash;
    public List<Character> founders = new List<Character>();
    private List<Character> _employees = new List<Character>();
    public List<Character> employees {
        get { return _employees; }
    }

    void Start() {
        StartCoroutine(PayYourDebts());
    }

    void Update() {
    }

    public void HireEmployee(Character employee) {
        _employees.Add(employee);
    }
    public void FireEmployee(Character employee) {
        _employees.Remove(employee);
    }

    IEnumerator PayYourDebts() {
        while(true) {
            // Pay debts
            yield return new WaitForSeconds(60);
        }
    }
}


