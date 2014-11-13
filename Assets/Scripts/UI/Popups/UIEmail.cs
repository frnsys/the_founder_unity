using UnityEngine;
using System.Collections;

// An email correspondence.
public class UIEmail : UIAlert {
    protected override void Extend(int amount) {
        amount = (amount/2) + 150;
        body.bottomAnchor.Set(window.transform, 0, -amount);
        body.topAnchor.Set(window.transform, 0, amount);
    }

    public void SetHeaders(string from, string to, string subject) {
        UILabel label = body.transform.Find("Content/Headers").GetComponent<UILabel>();

        label.text = "From: " + from + "\n"
                   + "To: " + to + "\n"
                   + "Subject: " + subject;
    }
}
