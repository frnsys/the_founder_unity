/*
 * The onboarding flow, shown after starting a new game.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIOnboarding : MonoBehaviour {
    public UIGrid cofounderGrid;

    private List<Founder> cofounders;
    private Founder selectedCofounder;

    void OnEnable() {
        cofounders = GameManager.Instance.narrativeManager.cofounders;
        foreach (Founder cf in cofounders) {
            // TO DO
            // Keep it simple (just labels) for now...but later make it more interesting.
            GameObject go = NGUITools.AddChild(cofounderGrid.gameObject, new GameObject());
            UILabel label = go.AddComponent<UILabel>();
            label.trueTypeFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            label.color = new Color(0f, 0f, 0f, 1f);
            label.autoResizeBoxCollider = true;
            BoxCollider bc = go.AddComponent<BoxCollider>();
            bc.isTrigger = true;
            go.name = cf.name;
            label.text = cf.name;

            UIEventListener.Get(go).onClick += SelectCofounder;
        }
        cofounderGrid.Reposition();
    }

    public void SelectCofounder(GameObject obj) {
        // This could be better too.

        // Unselect other cofounders.
        foreach (Transform t in cofounderGrid.gameObject.transform) {
            t.gameObject.GetComponent<UILabel>().color = new Color(0f, 0f, 0f, 1f);
        }

        selectedCofounder = cofounders.Find(x => x.name == obj.name);
        obj.GetComponent<UILabel>().color = new Color(0.18f, 0.67f, 0.23f, 1f);
    }

    public void ConfirmSelection() {
        // Add the cofounder.
        GameManager.Instance.narrativeManager.SelectCofounder(selectedCofounder);

        // Switch to the game scene.
        Application.LoadLevel("Game");
    }
}


