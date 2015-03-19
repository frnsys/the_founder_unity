using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UILobbying : UIFullScreenPager {
    private GameManager gm;
    public GameObject lobbyPrefab;

    void OnEnable() {
        gm = GameManager.Instance;
        LoadLobbies();
    }

    private void LoadLobbies() {
        ClearGrid();
        foreach (Lobby l in Lobby.LoadAll()) {
            GameObject lobbyItem = NGUITools.AddChild(grid.gameObject, lobbyPrefab);
            lobbyItem.GetComponent<UILobby>().lobby = l;
        }
        Adjust();
    }
}


