/*
 * The main menu.
 */

using UnityEngine;
using System;
using System.Collections;

public class UIMainMenu : Singleton<UIMainMenu> {
    public UIGrid menu;
    public GameObject newGameButton;
    public GameObject continueButton;
    public GameObject inputConfirmPrefab;

    void OnEnable() {
        // TO DO actually check if there's an existing game.
        bool existingGame = false;
        if (!existingGame) {
            continueButton.SetActive(false);
        }
        menu.Reposition();
    }

    public void NewGame() {
        // Show the input for the company name.
        UIInputConfirm ic = NGUITools.AddChild(gameObject, inputConfirmPrefab).GetComponent<UIInputConfirm>();
        UIEventListener.Get(ic.input.gameObject).onSubmit += StartNewGame;
        ic.bodyText = "What will your company be called?";

        UIEventListener.VoidDelegate yesAction = delegate(GameObject obj) {
            StartNewGame(ic.input.gameObject);
            ic.Close_();
        };

        UIEventListener.VoidDelegate noAction = delegate(GameObject obj) {
            ic.Close_();
        };

        UIEventListener.Get(ic.yesButton).onClick += yesAction;
        UIEventListener.Get(ic.noButton).onClick += noAction;
    }

    public void StartNewGame(GameObject obj) {
        string companyName = obj.GetComponent<UIInput>().value;

        // Load the starting game manager.
        GameManager.prefab = Resources.Load("GameManager") as GameObject;
        GameManager gm = GameManager.Instance;

        // Create the player company.
        gm.playerCompany = new Company(companyName);

        // Switch to the game scene.
        Application.LoadLevel("Game");
    }

    public void Continue() {
        // TO DO
        // Load the game and everything.
    }
}


