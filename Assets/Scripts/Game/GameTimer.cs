using UnityEngine;
using System.Collections;

[System.Serializable]
static class GameTimer {
    [SerializeField]
    private static float gameTimeElapsed = 0;

    [SerializeField]
    private static int pauses = 0;

    // Start the timer.
    public static IEnumerator Start() {
        // Increment the time elapsed *only* if not paused.
        while (true) {
            if (pauses == 0) {
                gameTimeElapsed += Time.deltaTime;
            }
            yield return null;
        }
    }

    public static bool paused {
        get { return pauses > 0; }
    }
    public static void Pause() {
        pauses++;
        Debug.Log(string.Format("Paused: {0}", pauses));
    }
    public static void Resume() {
        pauses--;
        if (pauses < 0)
            pauses = 0;
        Debug.Log(string.Format("Resumed: {0}", pauses));
    }
    public static void Reset() {
        pauses = 0;
    }

    public static IEnumerator Wait(float seconds) {
        float releaseTime = gameTimeElapsed + seconds;
        while (gameTimeElapsed < releaseTime) {
            yield return null; // wait 1 frame then check the time again...
        }
    }
}

