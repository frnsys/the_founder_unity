using UnityEngine;

public class UIBuffEffect : MonoBehaviour {
    public UILabel label;

    public void Set(StatBuff buff, string target) {
        string buffText = string.Format("Adds {0:F2} to {1}", buff.value, buff.name);

        if (target != null)
            buffText += string.Format(" for {0}", target);

        label.text = buffText;
    }
}


