
using UnityEngine;
using System;
using System.Collections;

public class UISettings : UIWindow {
    public UIButton musicButton;
    public UIButton fxButton;

    private Color activeColor = new Color(0.47f,0.88f,0.62f,1f);
    private Color inactiveColor = new Color(0.8f,0.8f,0.8f,1f);

    void OnEnable() {
        UpdateButton("Music", musicButton);
        UpdateButton("FX", fxButton);
    }

    public void ToggleMusic() {
        int val = PlayerPrefs.GetInt("Music", 1) == 1 ? 0 : 1;
        PlayerPrefs.SetInt("Music", val);
        UpdateButton("Music", musicButton);
        AudioManager.Instance.UpdatePrefs();
    }

    public void ToggleFX() {
        int val = PlayerPrefs.GetInt("FX", 1) == 1 ? 0 : 1;
        PlayerPrefs.SetInt("FX", val);
        UpdateButton("FX", fxButton);
        AudioManager.Instance.UpdatePrefs();
    }

    private void SetButtonColor(UIButton b, Color c) {
        b.defaultColor = c;
        b.pressed = c;
        b.hover = c;
        b.disabledColor = c;
    }

    private void UpdateButton(string key, UIButton button) {
        if (PlayerPrefs.GetInt(key) == 1)
            SetButtonColor(button, activeColor);
        else
            SetButtonColor(button, inactiveColor);
    }
}
