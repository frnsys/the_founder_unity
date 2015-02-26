using UnityEngine;

// TO DO this isn't being used now
public class AudioManager : MonoBehaviour {
    public AudioSource music;
    public AudioSource fx;

    void Start() {
        UpdatePrefs();
    }

    public void UpdatePrefs() {
        music.mute = PlayerPrefs.GetInt("Music", 1) == 0;
        fx.mute    = PlayerPrefs.GetInt("FX",    1) == 0;
    }
}
