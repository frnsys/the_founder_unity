using UnityEngine;
using System.Collections;


public class OfficeTile : OwnedTile {
    public Piece developing;
    public float progress;
    public float developmentCost;

    public bool developPiece(Company c) {
        progress += c.productivity;
        if (progress >= developmentCost) {
            progress = 0;
            return true;
        }
        return false;
    }
}
