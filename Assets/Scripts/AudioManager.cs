using UnityEngine;

public class AudioManager : Singleton<AudioManager> {
    public AudioSource music;
    public AudioSource fx;

    // Disable the constructor.
    protected AudioManager() {}

    void Awake() {
        DontDestroyOnLoad(gameObject);

        music = gameObject.AddComponent<AudioSource>();
        fx = gameObject.AddComponent<AudioSource>();

        comboAudioClip = Resources.Load<AudioClip>("Sounds/Combo");
        laborAudioClips = Resources.LoadAll<AudioClip>("Sounds/Labor/");
        employeeTouchClips = Resources.LoadAll<AudioClip>("Sounds/Employee/");

        menuClip = Resources.Load<AudioClip>("Sounds/Music/Menu");

        UpdatePrefs();
    }

    public void UpdatePrefs() {
        music.mute = PlayerPrefs.GetInt("Music", 1) == 0;
        fx.mute    = PlayerPrefs.GetInt("FX",    1) == 0;
    }

    public AudioClip[] laborAudioClips;
    public AudioClip comboAudioClip;
    public AudioClip[] employeeTouchClips;
    public AudioClip menuClip;

    public void PlayComboFX() {
        fx.PlayOneShot(comboAudioClip);
    }

    public void PlayLaborFX() {
        fx.PlayOneShot(laborAudioClips[Random.Range(0, laborAudioClips.Length)]);
    }

    public void PlayEmployeeTouchedFX() {
        fx.PlayOneShot(employeeTouchClips[Random.Range(0, employeeTouchClips.Length)]);
    }

    public void PlayMenuMusic() {
        music.clip = menuClip;
        music.Play();
    }
}
