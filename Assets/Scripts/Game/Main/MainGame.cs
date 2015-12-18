using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainGame : MonoBehaviour {
    // Much credit due to github.com/dgkanatsios/matchthreegame for guidance
    static public event System.Action Done;

    public int totalTurns;
    public int turnsLeft;

    private Company company;
    private int workUnit = 10; // TODO balance this

    private int rows;
    private int cols;
    private float gridItemSize = 1f;
    private float animationDuration = 0.2f;

    public GameObject board;
    public GameObject[] productTypePrefabs;
    public GameObject[] influencerPrefabs;
    public GameObject outragePrefab;
    public GameObject happyPrefab;
    public GameObject bugPrefab;
    public GameObject blockPrefab;
    public GameObject[,] grid;

    public GameObject ui;
    public UIProgressBar turnsBar;
    public GameObject resultPrefab;

    private GameObject hitGo;

    // for testing
    void Start() {
        StartGame();
    }

    public void StartGame() {
        company = GameManager.Instance.playerCompany;
        state = GameState.None;

        // At minimum, 2 turns
        //totalTurns = Math.Max(2, (int)Math.Floor(company.productivity/workUnit));
        // testing
        totalTurns = 10;
        turnsLeft = totalTurns;

        InitGrid();
        UpdateUI();
    }

    // Show a float-up text bit at the specified position
    private void ShowResultAt(Vector2 pos, string text) {
        pos.x += gridItemSize/2;

        // Get correct positioning for NGUI
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        screenPos.x -= Screen.width/2f;
        screenPos.y -= Screen.height/2f;

        GameObject go = NGUITools.AddChild(ui, resultPrefab);
        go.GetComponent<UILabel>().text = text;
        go.transform.localPosition = screenPos;
        go.transform.localPositionTo(animationDuration, screenPos + new Vector3(0, 32f, 0));
        TweenAlpha.Begin(go, 1f, 0f);
    }

    private void InitGrid() {
        // TODO these depend on number of locations owned or something
        rows = 5;
        cols = 5;
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

    private enum GameState {
        None,
        Selecting,
        Animating
    }
    private GameState state;

    void Update() {
        if (state == GameState.None) {
            // Click/tap
            if (Input.GetMouseButtonDown(0)) {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null) {
                    hitGo = hit.collider.gameObject;
                    state = GameState.Selecting;
                }
            }
        } else if (state == GameState.Selecting) {
            // Drag
            if (Input.GetMouseButton(0)) {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hitGo != hit.collider.gameObject) {
                    if (!ValidNeighbors(hit.collider.gameObject.GetComponent<Piece>(),
                        hitGo.GetComponent<Piece>())) {
                        state = GameState.None;
                    } else {
                        state = GameState.Animating;
                        StartCoroutine(FindMatchesAndCollapse(hit));
                    }
                }
            }
        }
    }

    private IEnumerator FindMatchesAndCollapse(RaycastHit2D hit2) {
        var hitGo2 = hit2.collider.gameObject;
        Swap(hitGo, hitGo2);

        // Swap in the rendered grid
        hitGo.transform.positionTo(animationDuration, hitGo2.transform.position);
        hitGo2.transform.positionTo(animationDuration, hitGo.transform.position);
        yield return new WaitForSeconds(animationDuration);

        // For now, assume a successful turn
        turnsLeft--;
        state = GameState.None;

        ShowResultAt(hitGo.transform.localPosition, "$24,000");
        UpdateUI();
    }

    // Swap in the grid
    private void Swap(GameObject g1, GameObject g2) {
        Piece p1 = g1.GetComponent<Piece>();
        Piece p2 = g2.GetComponent<Piece>();

        GameObject tmp = grid[p1.x, p1.y];
        grid[p1.x, p1.y] = grid[p2.x, p2.y];
        grid[p2.x, p2.y] = tmp;

        p1.SwapWith(p2);
    }

    private void UpdateUI() {
        turnsBar.value = (float)turnsLeft/totalTurns;
    }


    // Check if two pieces are valid neighbors,
    // i.e. not diagonal neighbors
    private bool ValidNeighbors(Piece p1, Piece p2) {
        return (p1.x == p2.x || p1.y == p2.y) && Math.Abs(p1.x - p2.x) <= 1 && Math.Abs(p1.y - p2.y) <= 1;
    }
}
