/*
 * Product Item
 * ===================
 *
 * A single product item,
 * to keep track of its progress
 * and revenue.
 *
 */

using UnityEngine;
using System.Collections;

public class UIProduct: MonoBehaviour {
    private Product product_;
    public Product product {
        get { return product_; }
        set {
            product_ = value;
            name.text = product_.name;
            points.text = product_.points.ToString() + "pp";

            if (revenue)
                revenue.text = "$" + product_.revenueEarned.ToString();

            if (progress)
                progress.value = product_.progress;
        }
    }

    public UILabel name;
    public UILabel revenue;
    public UILabel points;
    public UIProgressBar progress;
}


