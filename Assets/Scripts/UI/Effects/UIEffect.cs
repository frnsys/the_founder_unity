using UnityEngine;

public class UIEffect : MonoBehaviour {
    public UILabel label;

    public void SetUnlock(string name) {
        label.text = "Unlocks " + name;
    }

    public void SetSpecial(EffectSet.Special effect) {
        string text = "";
        switch (effect) {
            case EffectSet.Special.Immortal:
                text = "Grants you immortality";
                break;
            case EffectSet.Special.Cloneable:
                text = "Allows cloning of your workers";
                break;
            case EffectSet.Special.Prescient:
                text = "65% chance of correctly predicting economic fluctations";
                break;
            case EffectSet.Special.WorkerInsight:
                text = "Provides detailed metrics of potential hires";
                break;
            case EffectSet.Special.Automation:
                text = "Allows hiring of automated workers";
                break;
            case EffectSet.Special.FounderAI:
                text = "Win the game";
                break;
        }
        label.text = text;
    }

    public void Set(string text) {
        label.text = text;
    }
}


