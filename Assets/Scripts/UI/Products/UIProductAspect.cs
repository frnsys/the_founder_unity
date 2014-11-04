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
    private ProductAspect aspect_;
    public ProductAspect aspect {
        get { return aspect_; }
        set {
            aspect_ = value;
            label.text = aspect_.ToString();
            description.text = aspect_.description;
            points.text = aspect_.points.ToString() + "PP";
        }
    }

    public UILabel label;
    public UILabel description;
    public UILabel points;
}


