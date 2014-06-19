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
    public List<Office> offices = new List<Office>();

    void Start() {
        StartCoroutine(PayYourDebts());
    }

    void Update() {
    }

    public bool HireEmployee(Character employee, Office office) {
        if (offices.Contains(office)) {
            List<Character> officeEmployees = employees.FindAll(i => i.office = office);
            if (officeEmployees.Count < office.size) {
                _employees.Add(employee);
                return true;
            }
        }
        return false;
    }
    public void FireEmployee(Character employee) {
        _employees.Remove(employee);
    }

    public void Pay() {
        // Pay employees.
        foreach (Character employee in employees) {
            cash -= employee.salary;
        }

        // Pay rent.
        foreach (Office office in offices) {
            cash -= office.rent;
        }
    }

    IEnumerator PayYourDebts() {
        while(true) {
            // Pay debts
            yield return new WaitForSeconds(60);
        }
    }
}


