using UnityEngine;
using System.Collections;

public class UIExit : MonoBehaviour {
    public GameObject display;

    void OnClick() {
        UIManager.Instance.Confirm("Are you sure you want to exit? Your progress will be saved.", delegate() {
            GameManager.Instance.SaveGame();
            Destroy(GameManager.Instance.gameObject);
            Application.LoadLevel("MainMenu");
        }, null);
    }

    void Update() {
        if (display != null)
            UIAnimator.Rotate(display);
    }
}
