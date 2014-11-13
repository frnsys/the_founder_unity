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
    public GameObject confirmPrefab;
    public GameObject inputConfirmPrefab;

    // TO DO actually check if there's an existing game.
    private bool existingGame = false;

    void OnEnable() {
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

    public void StartNewGame(GameObject go) {
        string companyName = go.GetComponent<UIInput>().value;

        // If a game exists, confirm overwrite.
        if (existingGame) {
            UIConfirm confirm = NGUITools.AddChild(gameObject, confirmPrefab).GetComponent<UIConfirm>();
            confirm.bodyText = "Are you sure you want to start a new game? This will overwrite your existing one.";

            UIEventListener.VoidDelegate yesAction = delegate(GameObject obj) {
                BeginNewGame(companyName);
            };

            UIEventListener.VoidDelegate noAction = delegate(GameObject obj) {
                confirm.Close_();
            };

            UIEventListener.Get(confirm.yesButton).onClick += yesAction;
            UIEventListener.Get(confirm.noButton).onClick += noAction;

        } else {
            BeginNewGame(companyName);
        }
    }

    private void BeginNewGame(string companyName) {

        // Load the starting game manager.
        GameManager.prefab = Resources.Load("GameManager") as GameObject;
        GameManager gm = GameManager.Instance;

        // Create the player company.
        gm.playerCompany = new Company(companyName);

        // Switch to the onboarding scene.
        Application.LoadLevel("Onboarding");
    }

    public void Continue() {
        // TO DO
        // Load the game and everything.
    }
}


