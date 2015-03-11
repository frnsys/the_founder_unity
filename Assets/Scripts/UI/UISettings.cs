
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
        Toggle("Music", fxButton);
    }

    public void ToggleFX() {
        Toggle("FX", fxButton);
    }

    private void Toggle(string key, UIButton button) {
        int val = PlayerPrefs.GetInt(key, 1) == 1 ? 0 : 1;
        PlayerPrefs.SetInt(key, val);
        UpdateButton(key, button);
        AudioManager.Instance.UpdatePrefs();
    }

    private void UpdateButton(string key, UIButton button) {
        if (PlayerPrefs.GetInt(key) == 1)
            SetButtonColor(button, activeColor);
        else
            SetButtonColor(button, inactiveColor);
    }

    private void SetButtonColor(UIButton b, Color c) {
        b.defaultColor = c;
        b.pressed = c;
        b.hover = c;
        b.disabledColor = c;
    }
}
