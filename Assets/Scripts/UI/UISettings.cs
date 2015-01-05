
using UnityEngine;
using System;
using System.Collections;

public class UISettings : UIWindow {
    public UIButton musicButton;
    public UIButton fxButton;

    private Color activeColor = new Color(0.47f,0.88f,0.62f,1f);
    private Color inactiveColor = new Color(0.8f,0.8f,0.8f,1f);

    public void ToggleMusic() {
        int val = PlayerPrefs.GetInt("Music", 1) == 1 ? 0 : 1;
        PlayerPrefs.SetInt("Music", val);
        if (val == 1)
            SetButtonColor(musicButton, activeColor);
        else
            SetButtonColor(musicButton, inactiveColor);

        Camera.main.gameObject.GetComponent<AudioManager>().UpdatePrefs();
    }

    public void ToggleFX() {
        int val = PlayerPrefs.GetInt("FX", 1) == 1 ? 0 : 1;
        PlayerPrefs.SetInt("FX", val);
        if (val == 1)
            SetButtonColor(fxButton, activeColor);
        else
            SetButtonColor(fxButton, inactiveColor);

        Camera.main.gameObject.GetComponent<AudioManager>().UpdatePrefs();
    }

    private void SetButtonColor(UIButton b, Color c) {
        b.defaultColor = c;
        b.pressed = c;
        b.hover = c;
        b.disabledColor = c;
    }
}
