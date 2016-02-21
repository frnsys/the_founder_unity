using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player {
    public enum Type {
        Human,
        AI
    }
    public Type type;
    public List<Tile> tiles;
    public List<Piece> pieces;
    public Company company;
    public AAICompany aicompany;

    public bool isAI {
        get { return type == Type.AI; }
    }

    public Player(Company c) {
        type = Type.Human;
        company = c;
        tiles = new List<Tile>();
        pieces = new List<Piece>();
    }

    public Player(AAICompany c) {
        type = Type.AI;
        aicompany = c;
        tiles = new List<Tile>();
        pieces = new List<Piece>();
    }
}
