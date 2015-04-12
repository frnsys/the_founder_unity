/*
 * The onboarding flow, shown after starting a new game.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIOnboarding : MonoBehaviour {
    public UIGrid grid;
    public UILabel title;

    private GameManager gm;
    private List<Worker> cofounders;

    public GameObject verticalPrefab;
    public GameObject locationPrefab;
    public GameObject cofounderPrefab;

    public List<Vertical> startingVerticals;
    public List<Location> startingLocations;

    public GameObject continueButton;
    public GameObject backButton;

    private Worker selectedCofounder;
    private Vertical selectedVertical;
    private Location selectedLocation;

    private bool didShowVerticals = false;
    private bool didShowLocations = false;
    private bool didShowCofounders = false;

    private UIEventListener.VoidDelegate next;
    private UIEventListener.VoidDelegate back;

    public Color activeColor = new Color(0.18f, 0.67f, 0.23f, 1f);
    private Color inactiveColor = new Color(1f, 1f, 1f, 0f);

    void OnEnable() {
        gm = GameManager.Instance;
        cofounders = new List<Worker>(Resources.LoadAll<Worker>("Founders/Cofounders"));

        Intro();

        continueButton.SetActive(false);
        backButton.SetActive(false);
    }

    void Intro() {
        string[] messages = new string[] {
            "Can you believe it's already 2001? The dot-com bubble just burst, but I'm looking forward to the years ahead.",
            string.Format("A few stronger enterprises - {0}, {1}, {2}, and others - have managed to survive the bubble.",
                    gm.narrativeManager.ConceptHighlight("Kougle"),
                    gm.narrativeManager.ConceptHighlight("Coralzon"),
                    gm.narrativeManager.ConceptHighlight("Carrot Inc.")),
            string.Format("But their weakened state and the void left by mass bankruptcy means the business world is ripe for {0}.", gm.narrativeManager.SpecialHighlight("disruption")),
            "I have a good feeling about you - so I've given you some starting funds to take advantage of this time and build a powerful company yourself."
        };
        gm.narrativeManager.MentorMessages(messages, delegate(GameObject obj) {
            continueButton.SetActive(true);
            backButton.SetActive(true);
            VerticalSelection(null);
        });
    }

    void BackToMainMenu(GameObject obj) {
        Application.LoadLevel("MainMenu");
    }


    /*
     * ==========================
     * Vertical =================
     * ==========================
     */
    void VerticalSelection(GameObject obj) {
        title.text = "What kind of products\nwill you make?";
        back = BackToMainMenu;
        continueButton.GetComponent<UIButton>().isEnabled = false;

        ClearGrid();
        foreach (Vertical v in startingVerticals) {
            GameObject go = NGUITools.AddChild(grid.gameObject, verticalPrefab);
            UIOnboardingVertical ov = go.GetComponent<UIOnboardingVertical>();
            ov.vertical = v;
            UIEventListener.Get(go).onClick += SelectVertical;

            if (selectedVertical == v) {
                ov.background.color = activeColor;
                continueButton.GetComponent<UIButton>().isEnabled = true;
            }
        }
        grid.Reposition();

        next = ConfirmVertical;

        // If no game object is present, it's because we're calling it manually.
        // Otherwise, it is a "back" event.
        if (!didShowVerticals) {
            string[] messages = new string[] {
                "First you have to choose the kind of business you want to create.",
                "Remember that a lot of companies start doing one thing and grow to do many."
            };
            gm.narrativeManager.MentorMessages(messages);
            didShowVerticals = true;
        }
    }

    void SelectVertical(GameObject obj) {
        foreach (Transform t in grid.transform) {
            t.GetComponent<UIOnboardingVertical>().background.color = inactiveColor;
        }

        UIOnboardingVertical ov = obj.GetComponent<UIOnboardingVertical>();
        selectedVertical = ov.vertical;
        ov.background.color = activeColor;
        continueButton.GetComponent<UIButton>().isEnabled = true;
    }

    void ConfirmVertical(GameObject obj) {
        if (selectedVertical == null)
            return;

        LocationSelection(null);
    }

    /*
     * ==========================
     * Location =================
     * ==========================
     */
    void LocationSelection(GameObject obj) {
        title.text = "Where will your HQ\nbe located?";
        back = VerticalSelection;
        continueButton.GetComponent<UIButton>().isEnabled = false;

        ClearGrid();
        foreach (Location l in startingLocations) {
            GameObject go = NGUITools.AddChild(grid.gameObject, locationPrefab);
            UIOnboardingLocation ov = go.GetComponent<UIOnboardingLocation>();
            ov.location = l;
            UIEventListener.Get(go).onClick += SelectLocation;

            if (selectedLocation == l) {
                ov.background.color = activeColor;
                continueButton.GetComponent<UIButton>().isEnabled = true;
            }
        }
        grid.Reposition();

        next = ConfirmLocation;

        if (!didShowLocations) {
            string[] messages = new string[] {
                "Next, where do you want to base your company?",
                "Locations can have different bonuses and access to different markets."
            };
            gm.narrativeManager.MentorMessages(messages);
            didShowLocations = true;
        }
    }

    void SelectLocation(GameObject obj) {
        foreach (Transform t in grid.transform) {
            t.GetComponent<UIOnboardingLocation>().background.color = inactiveColor;
        }

        UIOnboardingLocation ov = obj.GetComponent<UIOnboardingLocation>();
        selectedLocation = ov.location;
        ov.background.color = activeColor;
        continueButton.GetComponent<UIButton>().isEnabled = true;
    }

    void ConfirmLocation(GameObject obj) {
        if (selectedLocation == null)
            return;

        CofounderSelection(null);
    }

    /*
     * ==========================
     * Cofounder ================
     * ==========================
     */
    void CofounderSelection(GameObject obj) {
        title.text = "Who will be your cofounder?";
        continueButton.transform.Find("Label").GetComponent<UILabel>().text = "Next";
        continueButton.GetComponent<UIButton>().isEnabled = false;
        back = LocationSelection;

        ClearGrid();
        foreach (Worker f in cofounders) {
            GameObject go = NGUITools.AddChild(grid.gameObject, cofounderPrefab);
            UIOnboardingCofounder ov = go.GetComponent<UIOnboardingCofounder>();
            ov.cofounder = f;
            UIEventListener.Get(go).onClick += SelectCofounder;

            if (selectedCofounder == f) {
                ov.background.color = activeColor;
                continueButton.GetComponent<UIButton>().isEnabled = true;
            }
        }
        grid.Reposition();

        UIEventListener.Get(continueButton).onClick += ConfirmCofounder;
        next = ConfirmCofounder;

        if (!didShowCofounders) {
            string[] messages = new string[] {
                "Finally, there's no way you'll be able to do this alone. You need a cofounder.",
                "Picking the cofounder is one of the most important decisions for a business.",
                "They vary in what they can bring to the table. You want to pick one that reflects your own style and skillset."
            };
            gm.narrativeManager.MentorMessages(messages);
            didShowCofounders = true;
        }
    }

    void SelectCofounder(GameObject obj) {
        foreach (Transform t in grid.transform) {
            t.GetComponent<UIOnboardingCofounder>().background.color = inactiveColor;
        }

        UIOnboardingCofounder ov = obj.GetComponent<UIOnboardingCofounder>();
        selectedCofounder = ov.cofounder;
        ov.background.color = activeColor;
        continueButton.GetComponent<UIButton>().isEnabled = true;
    }

    void ConfirmCofounder(GameObject obj) {
        if (selectedCofounder == null)
            return;

        ConfirmationScreen();
    }

    void ConfirmationScreen() {
        title.text = gm.playerCompany.name;
        back = CofounderSelection;

        // Show current selections.
        GameObject go;
        ClearGrid();

        go = NGUITools.AddChild(grid.gameObject, verticalPrefab);
        go.GetComponent<UIOnboardingVertical>().vertical = selectedVertical;

        go = NGUITools.AddChild(grid.gameObject, locationPrefab);
        go.GetComponent<UIOnboardingLocation>().location = selectedLocation;

        go = NGUITools.AddChild(grid.gameObject, cofounderPrefab);
        go.GetComponent<UIOnboardingCofounder>().cofounder = selectedCofounder;

        grid.Reposition();

        next = StartGame;
        continueButton.transform.Find("Label").GetComponent<UILabel>().text = "Found " + gm.playerCompany.name;
    }

    void StartGame(GameObject obj) {
        gm.InitializeGame(selectedCofounder, selectedLocation, selectedVertical);

        // Switch to the game scene.
        Application.LoadLevel("Game");
    }

    void ClearGrid() {
        while (grid.transform.childCount > 0)
            NGUITools.Destroy(grid.transform.GetChild(0).gameObject);
    }

    public void Back() {
        if (back != null)
            back(null);
    }

    public void Next() {
        if (next != null)
            next(null);
    }
}


