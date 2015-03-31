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

        laborAudioClips = Resources.LoadAll<AudioClip>("Sounds/Labor/Make");
        laborHitAudioClips = Resources.LoadAll<AudioClip>("Sounds/Labor/Hit");

        menuClip = Resources.Load<AudioClip>("Sounds/Music/Menu");

        UpdatePrefs();
    }

    public void UpdatePrefs() {
        music.mute = PlayerPrefs.GetInt("Music", 1) == 0;
        fx.mute    = PlayerPrefs.GetInt("FX",    1) == 0;
    }

    public AudioClip[] laborAudioClips;
    public AudioClip[] laborHitAudioClips;
    public AudioClip menuClip;

    public void PlayLaborFX() {
        fx.PlayOneShot(laborAudioClips[Random.Range(0, laborAudioClips.Length)]);
    }
    public void PlayLaborHitFX() {
        fx.PlayOneShot(laborHitAudioClips[Random.Range(0, laborHitAudioClips.Length)]);
    }

    public void PlayPowerupHitFX() {
        fx.PlayOneShot(Resources.Load<AudioClip>("Sounds/Product/shell_hit"));
    }

    public void PlayPlayerDeadFX() {
        fx.PlayOneShot(Resources.Load<AudioClip>("Sounds/Product/shell_destroy"));
    }

    public void PlayPlayerHitFX() {
        fx.PlayOneShot(Resources.Load<AudioClip>("Sounds/Employee/hit"));
    }

    public void PlayMenuMusic() {
        music.clip = menuClip;
        music.loop = true;
        music.Play();
    }
}
