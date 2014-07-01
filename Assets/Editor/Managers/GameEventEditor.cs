using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[InitializeOnLoad]
public class GameEventManager : EditorWindow {

    // Opens the window
    [MenuItem("Managers/Game Event Manager")]
    static void Init() {
        GameEventManager window = (GameEventManager)EditorWindow.CreateInstance(typeof(GameEventManager));
        window.Show();
    }

    static GameEventManager() {
        EditorApplication.update += RunOnce;
    }

    static void RunOnce() {
        gameEvents = new List<GameEvent>(Resources.LoadAll<GameEvent>("GameEvents"));
        folds = new Dictionary<GameEvent, bool>();

        // By default, all foldouts are closed.
        foreach (GameEvent e in gameEvents) {
            folds.Add(e, false);
        }

        EditorApplication.update -= RunOnce;
    }

    static private List<GameEvent> gameEvents;
    private List<GameEvent> searchResults;
    private string query;
    private string query_;

    // Keep track of foldout states.
    static private Dictionary<GameEvent, bool> folds;

    void OnGUI() {
        // Search bar.
        query = EditorGUILayout.TextField("Search: ", query);
        searchResults = gameEvents.FindAll(i => i.name.Contains(query));

        foreach (GameEvent e in searchResults) {
            folds[e] = EditorGUILayout.Foldout(folds[e], e.name);
            if (folds[e]) {
                // Example.
                EditorGUILayout.LabelField(e.name);
            }
        }

    }

}
