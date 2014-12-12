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

            // Create the 3d models and position them.
            GameObject model = NGUITools.AddChild(label.gameObject, GameManager.Instance.config.employeeModelPrefab);
            model.transform.localScale = new Vector3(40, 40, 40);
            model.transform.localPosition = new Vector3(-120, 0, 0);

            model.transform.Find("Cone").GetComponent<SkinnedMeshRenderer>().material.mainTexture = cf.texture;

            // Set the label so that the model can be clicked on.
            label.pivot = UIWidget.Pivot.Bottom;
            label.height = 150;

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


