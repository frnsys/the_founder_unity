using UnityEngine;
using System.Collections;

public class UINewProductFlow : MonoBehaviour {
    private GameManager gm;

    private enum Aspect {
        PRODUCTTYPE,
        INDUSTRY,
        MARKET,
        COMPLETE
    }
    private Aspect aspect = Aspect.PRODUCTTYPE;

    public UILabel aspectLabel;
    public UITexture background;
    public UICenterOnChild grid;
    public UIScrollView scrollView;
    public UIButton selectButton;

    private ProductType productType;
    private Industry industry;
    private Market market;

    void OnEnable() {
        gm = GameManager.Instance;
        background.color = new Color(0.61f,0.067f,0.57f,1f);

        grid.onFinished = OnCenter;
    }

    private void OnCenter() {
        selectButton.isEnabled = true;
        Debug.Log(grid.centeredObject);
    }

    void Update() {
        if (scrollView.isDragging) {
            // Disable the select button
            // while dragging.
            selectButton.isEnabled = false;
        }
    }


    public void Select() {
        switch (aspect) {
            case Aspect.PRODUCTTYPE:
                aspect = Aspect.INDUSTRY;
                aspectLabel.text = "INDUSTRY";
                background.color = new Color(1f,1f,1f,1f);
                break;
            case Aspect.INDUSTRY:
                aspect = Aspect.MARKET;
                aspectLabel.text = "MARKET";
                background.color = new Color(1f,0.69f,1f,1f);
                break;
            case Aspect.MARKET:
                aspect = Aspect.COMPLETE;
                background.color = new Color(0.2f,0.69f,0.7f,1f);

                // Hide the aspect label.
                aspectLabel.gameObject.SetActive(false);
                break;
        }
    }
}


