using System;
using UnityEngine;
using System.Collections;

public class OwnedTile : Tile {
    public Player owner;
    public Player capturingPlayer;

    public int baseCost = 5;
    public int capturedCost = 0;

    public int CostForPlayer(Player p) {
        // TODO finish

        if (p == owner) {
            return 0;

        // continue capturing
        } else if (p == capturingPlayer) {
            return baseCost - capturedCost;

        // have to undo existing progress
        } else {
            return baseCost + capturedCost;
        }
    }

    public bool Capture(ProductPiece p) {
        // TODO finish
        if (p.owner == capturingPlayer) {
            capturedCost = Math.Min(capturedCost + p.quality, baseCost);
            if (capturedCost >= baseCost) {
                capturedCost = 0;
                return true;
            }
        }
        return false;
    }
}
