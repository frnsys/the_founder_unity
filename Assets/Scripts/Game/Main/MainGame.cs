using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainGame : MonoBehaviour {
    static public event System.Action Done;

    public int totalTurns;
    public int turns;

    private Company company;
    private int workUnit = 10; // TODO balance this

    private int rows;
    private int cols;
    private float gridItemSize = 0.8f;

    public GameObject board;
    public GameObject[] productTypePrefabs;
    public GameObject[] influencerPrefabs;
    public GameObject outragePrefab;
    public GameObject happyPrefab;
    public GameObject bugPrefab;
    public GameObject blockPrefab;
    public GameObject[,] grid;

    // for testing
    void Start() {
        StartGame();
    }

    public void StartGame() {
        company = GameManager.Instance.playerCompany;
        turns = 0;

        // At minimum, 2 turns
        totalTurns = Math.Max(2, (int)Math.Floor(company.productivity/workUnit));

        InitGrid();
    }

    private void InitGrid() {
        // TODO these depend on number of locations owned or something
        rows = 4;
        cols = 4;
        grid = new GameObject[rows, cols];

        Vector2 bottomRight = Camera.main.ViewportToWorldPoint(new Vector2(1,0));
        Vector2 topLeft = Camera.main.ViewportToWorldPoint(new Vector2(0,1));

        float worldWidth = bottomRight.x - topLeft.x;
        float worldHeight = topLeft.y - bottomRight.y;
        float gridWidth = (cols-1) * gridItemSize;
        float gridHeight = (rows-1) * gridItemSize;

        Vector2 startPos = new Vector2(-gridWidth/2, -gridHeight/2);

        for (int r=0; r<rows; r++) {
            for (int c=0; c<cols; c++) {
                GameObject p = RandomPiece();

                // Reselect piece as necessary, so we don't start with
                // three in a row/col of the same type
                while (c >= 2 && grid[r, c-1].GetComponent<Piece>().Equals(p)
                        && grid[r, c-2].GetComponent<Piece>().Equals(p)) {
                    p = RandomPiece();
                }
                while (r >= 2 && grid[r-1, c].GetComponent<Piece>().Equals(p)
                        && grid[r-2, c].GetComponent<Piece>().Equals(p)) {
                    p = RandomPiece();
                }

                GameObject piece = Instantiate(p, startPos + new Vector2(c * gridItemSize, r * gridItemSize), p.transform.rotation) as GameObject;
                piece.GetComponent<Piece>().Setup(p.GetComponent<Piece>().type, r, c);
                piece.transform.parent = board.transform;
                grid[r, c] = piece;
            }
        }
    }

    private GameObject RandomPiece() {
        // TODO balance this
        if (UnityEngine.Random.value <= Mathf.Max(0.02f, -company.opinion.value/100)) {
            return outragePrefab;

        // TODO balance this
        } else if (UnityEngine.Random.value < Mathf.Min(0.15f, company.charisma/200)) {
            float roll = UnityEngine.Random.value;
            if (roll <= 0.55f)
                return influencerPrefabs[0];
            else if (roll <= 0.9f)
                return influencerPrefabs[1];
            else
                return influencerPrefabs[2];
        } else {
            return productTypePrefabs[UnityEngine.Random.Range(0, productTypePrefabs.Length)];
        }
    }
}
