using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class MusicManager : Singleton<MusicManager> {
    // Disable the constructor.
    protected MusicManager() {}

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}
