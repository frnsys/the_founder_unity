using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainGame : MonoBehaviour {
    static public event System.Action Done;

    public int totalTurns;
    public int turns;

    private Company company;
    private int workUnit = 10; // TODO balance this

    public void StartGame() {
        company = GameManager.Instance.playerCompany;
        turns = 0;

        // At minimum, 2 turns
        totalTurns = Math.Max(2, (int)Math.Floor(company.productivity/workUnit));
    }
}
