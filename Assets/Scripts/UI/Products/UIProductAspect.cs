/*
 * Product Aspect Item
 * ===================
 *
 * A single product aspect (Product Type,
 * Industry, or Market) for part of the
 * new product flow.
 *
 */

using UnityEngine;
using System.Collections;

public class UIProductAspect : MonoBehaviour {
    // TO DO replace this eventually
    private ProductType aspect_;
    public ProductType aspect {
        get { return aspect_; }
        set {
            aspect_ = value;
            label.text = aspect_.ToString();
            description.text = aspect_.description;
            // TO DO this should be required infrastructure
            //points.text = aspect_.points.ToString() + "PP";
        }
    }

    public UILabel label;
    public UILabel description;
    public UILabel points;
}


