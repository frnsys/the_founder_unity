using UnityEngine;

public class UIBuffEffect : MonoBehaviour {
    public UILabel label;
    public Color debuffColor;
    public Color buffColor;
    public Color disabledColor;

    public void Set(StatBuff buff, string target) {
        string buffText = "";

        if (buff.value <= 0) {
            label.color = debuffColor;
        } else {
            buffText += "+";
            label.color = buffColor;
        }

        buffText += string.Format("{0:F2}", buff.value);
        buffText += " to " + buff.name;

        if (target != null)
            buffText += " for " + target;

        label.text = buffText;
    }

    // Grey out this effect to indicate that it is inactive.
    public void Disable() {
        label.color = disabledColor;
    }
}


