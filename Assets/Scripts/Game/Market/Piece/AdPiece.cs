using UnityEngine;
using System.Collections;

public class AdPiece : Piece {
    public AdPiece(Player p) : base(p) {}

    public void hypeTile(IncomeTile tile) {
        tile.bonus += 3;
    }
}
