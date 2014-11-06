using UnityEngine;

public class UIBuffEffect : MonoBehaviour {
    public UILabel label;
    public UILabel typeLabel;
    public UITexture typeIcon;
    public Texture buffTexture;
    public Texture debuffTexture;

    // TO DO
    // make icon public and replaceable.

    public void Set(StatBuff buff, string target) {
        string buffText = "";

        if (buff.value <= 0) {
            typeLabel.text = "DEBUFF";
            typeLabel.color = new Color(0.99f, 0.05f, 0.11f, 1f);
            typeIcon.mainTexture = debuffTexture;
        } else {
            typeLabel.text = "BUFF";
            buffText += "+";
            typeLabel.color = new Color(0.18f, 0.67f, 0.23f, 1f);
            typeIcon.mainTexture = buffTexture;
        }

        if (buff.type == BuffType.ADD) {
            buffText += buff.value.ToString();
        } else {
            buffText += buff.value.ToString() + "x";
        }

        buffText += " to " + buff.name;
        buffText += " for " + target;

        label.text = buffText;
    }
}


