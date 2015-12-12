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
    public GameObject infoOverlay;
    public UILabel infoLabel;
    public UILabel loadingLabel;

    void OnEnable() {
        if (!GameData.SaveExists) {
            continueButton.SetActive(false);
        }
        infoOverlay.SetActive(false);
        menu.Reposition();
        AudioManager.Instance.PlayMenuMusic();
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
        if (GameData.SaveExists) {
            UIConfirm confirm = NGUITools.AddChild(gameObject, confirmPrefab).GetComponent<UIConfirm>();
            confirm.bodyText = "Are you sure you want to start a new game? This will overwrite your existing one.";

            UIEventListener.VoidDelegate yesAction = delegate(GameObject obj) {
                confirm.Close_();
                StartCoroutine(BeginNewGame(companyName));
            };

            UIEventListener.VoidDelegate noAction = delegate(GameObject obj) {
                confirm.Close_();
            };

            UIEventListener.Get(confirm.yesButton).onClick += yesAction;
            UIEventListener.Get(confirm.noButton).onClick += noAction;

        } else {
            StartCoroutine(BeginNewGame(companyName));
        }
    }

    // this has to be a coroutine so the loading message actually shows up...why??
    private IEnumerator BeginNewGame(string companyName) {
        infoOverlay.SetActive(true);
        infoLabel.text = "Creating the universe...";
        yield return new WaitForSeconds(1);

        // Setup the game data.
        GameData data = GameData.New(companyName);
        GameManager.Instance.Load(data);

        // Switch to the onboarding scene.
        Application.LoadLevel("Onboarding");
    }

    public IEnumerator Continue() {
        infoLabel.text = "Repeating history...";
        infoOverlay.SetActive(true);
        yield return new WaitForSeconds(1);

        // Load the game and everything.
        UIManager.Reset();
        UIOfficeManager.Reset();
        GameManager.Reset();
        GameTimer.Reset();
        GameManager.Instance.Load(GameData.Load());

        // Switch to the game scene.
        Application.LoadLevel("Game");
    }

    public void Settings(GameObject settingsPrefab) {
        GameObject window = NGUITools.AddChild(gameObject, settingsPrefab);
        window.GetComponent<UIWidget>().SetAnchor(gameObject, 0, 0, 0, 0);
    }
}


