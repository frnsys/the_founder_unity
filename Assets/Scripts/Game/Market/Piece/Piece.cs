using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {
    public Player owner;
    public int quality;

    public Piece(Player p) {
        owner = p;
        owner.pieces.Add(this);
    }

    public Piece Battle(Piece other) {
        // two pieces "battle", the loser gets destroyed
        float prob_win = ((float)quality/(quality + other.quality));
        if (Random.value <= prob_win) {
            other.Remove();
            return this;
        } else {
            this.Remove();
            return other;
        }
    }

    public void Remove() {
        owner.pieces.Remove(this);
        Destroy(gameObject);
    }
}
