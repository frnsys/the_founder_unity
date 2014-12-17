using UnityEngine;
using System.Collections;

// A guidance message from your mentor.
public class UIMentor : UIPopup {
    // Note: this doesn't pause the game. should it?

    public GameObject box;
    public GameObject shadow;
    public GameObject model;

    void OnEnable() {
        Show(box);
        Show(shadow);

        // TO DO animate in the mentor model

        // Position the model.
        Vector3 modelPos = box.transform.localPosition;
        float modelWidth =  model.transform.localScale.x * model.transform.Find("Cone").GetComponent<SkinnedMeshRenderer>().localBounds.size.x;
        modelPos.y -= box.GetComponent<UIWidget>().height/2;
        modelPos.x -= box.GetComponent<UIWidget>().width/2 + modelWidth;
        model.transform.localPosition = modelPos;
    }

    public void Hide() {
        base.Hide(box);
        base.Hide(shadow);

        // TO DO animate out the mentor model
    }

    public string message {
        set {
            box.transform.Find("Message").GetComponent<UILabel>().text = value;
        }
    }
}
