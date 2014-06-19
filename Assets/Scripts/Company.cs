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
            if (office.employees.Count < office.size) {
                _employees.Add(employee);
                office.employees.Add(employee);
                return true;
            }
        }
        return false;
    }
    public void FireEmployee(Character employee) {
        Office office = offices.Find(i => i.employees.Contains(employee));
        _employees.Remove(employee);
        if (office) {
            office.employees.Remove(employee);
        }
    }

    public Office OpenOffice(Location location) {
        // Check to see that there isn't already an office here.
        if (!offices.Find(i => i.location == location)) {
            GameObject gO = new GameObject("Skyscraper");
            gO.AddComponent<Office>();
            Office office = gO.GetComponent<Office>();
            office.location = location;
            offices.Add(office);
            return office;
        }
        return null;
    }
    public void CloseOffice(Office office) {
        offices.Remove(office);
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


