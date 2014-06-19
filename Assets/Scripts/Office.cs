using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Office : MonoBehaviour {
    public Location location;
    public Company company;
    public List<Character> employees = new List<Character>();

    public int size;
    public float baseRent;
    public float rent {
        get {
            if (location != null) {
                return baseRent * location.expensiveness;
            }
            return baseRent;
        }
    }

    public float happiness {
        get {
            float sum = 0f;
            foreach (Character employee in employees) {
                sum += employee.happiness.value;
            }
            return sum/employees.Count;
        }
    }

    public float productivity {
        get {
            float sum = 0f;
            foreach (Character employee in employees) {
                sum += employee.productivity.value;
            }
            return sum/employees.Count;
        }
    }


    void Start() {
    }

    void Update() {
    }
}


