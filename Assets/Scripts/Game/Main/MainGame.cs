using UnityEngine;
using System;
using System.Linq;
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
    private float moveAnimationDuration = 0.05f;
    private int minMatches = 3;

    public GameObject board;
    public GameObject[] productTypePrefabs;
    public GameObject[] influencerPrefabs;
    public GameObject outragePrefab;
    public GameObject happyPrefab;
    public GameObject bugPrefab;
    public GameObject blockPrefab;
    public GameObject emptyPrefab;
    public GameObject[,] grid;

    public Color productTypeColor;
    public Color influencerColor;
    public Color hazardColor;
    public Color emptyColor;
    public GameObject tilePrefab;

    public GameObject ui;
    public UIProgressBar turnsBar;
    public GameObject resultPrefab;

    private GameObject hitGo;
    private Vector2 startPos;

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

        startPos = new Vector2(-gridWidth/2, -gridHeight/2);

        for (int r=0; r<rows; r++) {
            for (int c=0; c<cols; c++) {
                PlaceEmptyPiece(r, c);
            }
        }
    }

    private GameObject PlaceEmptyPiece(int r, int c) {
        Vector2 pos = new Vector2(c * gridItemSize, r * gridItemSize);
        GameObject piece = Instantiate(emptyPrefab, startPos + pos, Quaternion.identity) as GameObject;
        GameObject tile = Instantiate(tilePrefab) as GameObject;
        tile.GetComponent<SpriteRenderer>().color = emptyColor;
        piece.name = piece.name.Replace("(Clone)", "");
        tile.transform.parent = piece.transform;
        tile.transform.localPosition = new Vector3(0,0,2);
        piece.GetComponent<Piece>().Setup(emptyPrefab.GetComponent<Piece>().type, r, c);
        piece.transform.parent = board.transform;
        grid[r, c] = piece;
        return piece;
    }

    private GameObject PlaceRandomPiece(int r, int c) {
        return CreateRandomPiece(r, c, new Vector2(c * gridItemSize, r * gridItemSize));
    }

    private GameObject SpawnRandomPiece(int r, int c) {
        return CreateRandomPiece(r, c, new Vector2(c * gridItemSize, rows * gridItemSize));
    }

    private GameObject CreateRandomPiece(int r, int c, Vector2 pos) {
        GameObject p = RandomPiece();
        Piece p_ = p.GetComponent<Piece>();

        // Reselect piece as necessary, so we don't start with
        // three in a row/col of the same type
        while (c >= 2
                && grid[r, c-1] != null && grid[r, c-1].GetComponent<Piece>().Equal(p_)
                && grid[r, c-2] != null && grid[r, c-2].GetComponent<Piece>().Equal(p_)) {
            p = RandomPiece();
            p_ = p.GetComponent<Piece>();
        }
        while (r >= 2
                && grid[r-1, c] != null && grid[r-1, c].GetComponent<Piece>().Equal(p_)
                && grid[r-2, c] != null && grid[r-2, c].GetComponent<Piece>().Equal(p_)) {
            p = RandomPiece();
            p_ = p.GetComponent<Piece>();
        }

        GameObject piece = Instantiate(p, startPos + pos, p.transform.rotation) as GameObject;
        GameObject tile = Instantiate(tilePrefab) as GameObject;
        switch (piece.GetComponent<Piece>().type) {
            case Piece.Type.ProductType:
                tile.GetComponent<SpriteRenderer>().color = productTypeColor;
                break;
            case Piece.Type.Bug:
                tile.GetComponent<SpriteRenderer>().color = hazardColor;
                break;
            case Piece.Type.Influencer:
                tile.GetComponent<SpriteRenderer>().color = influencerColor;
                break;
        }
        piece.name = piece.name.Replace("(Clone)", "");
        tile.transform.parent = piece.transform;
        tile.transform.localPosition = new Vector3(0,0,2);
        piece.GetComponent<Piece>().Setup(p.GetComponent<Piece>().type, r, c);
        piece.transform.parent = board.transform;
        grid[r, c] = piece;
        return piece;
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
            //return productTypePrefabs[UnityEngine.Random.Range(0, productTypePrefabs.Length)];
            return productTypePrefabs[UnityEngine.Random.Range(0, 5)];
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
        GameObject hitGo2 = hit2.collider.gameObject;

        Piece p1 = hitGo.GetComponent<Piece>();
        Piece p2 = hitGo2.GetComponent<Piece>();

        // If the two are product types and the same product type
        if (p1.type == Piece.Type.ProductType && p2.type == Piece.Type.ProductType) {
            if (hitGo.name == hitGo2.name) { // TODO more rigorous comparison
                // TODO Merge the two

            } else if (p1.stacked && p2.stacked) {
                // TODO create a new product
            }
        }

        Swap(hitGo, hitGo2);
        yield return new WaitForSeconds(animationDuration);

        // Check matches
        var matches1 = GetMatches(hitGo);
        var matches2 = GetMatches(hitGo2);
        IEnumerable<GameObject> totalMatches = matches1.matches.Union(matches2.matches).Distinct();

        if (totalMatches.Count() < minMatches)  {
            // Undo
            Swap(hitGo2, hitGo);
            yield return new WaitForSeconds(animationDuration);
        } else {
            // Only successful moves cost a turn
            turnsLeft--;
        }

        while (totalMatches.Count() >= minMatches) {
            //ShowResultAt(hitGo.transform.localPosition, "$24,000");
            foreach (GameObject go in totalMatches) {
                // Remove from grid
                grid[go.GetComponent<Piece>().row, go.GetComponent<Piece>().col] = null;
                Destroy(go);
            }

            // Get columns which will collapse and collapse them
            IEnumerable<int> columns = totalMatches.Select(go => go.GetComponent<Piece>().col).Distinct();
            Collapses collapses = Collapse(columns);

            int maxDistance = collapses.maxDistance;
            List<GameObject> newPieces = new List<GameObject>();

            // Create new pieces in empty spots
            foreach (int col in columns) {
                for (int row=0; row<rows; row++) {
                    if (grid[row, col] == null) {
                        GameObject piece = SpawnRandomPiece(row, col);
                        newPieces.Add(piece);

                        if (rows - row > maxDistance)
                            maxDistance = rows - row;
                    }
                }
            }

            foreach (GameObject go in newPieces.Union(collapses.pieces)) {
                go.transform.positionTo(moveAnimationDuration * maxDistance,
                        startPos + new Vector2(go.GetComponent<Piece>().col * gridItemSize, go.GetComponent<Piece>().row * gridItemSize));
            }

            yield return new WaitForSeconds(moveAnimationDuration * maxDistance);

            // Check for cascading effects
            totalMatches = GetMatches(newPieces).Union(GetMatches(collapses.pieces)).Distinct();
        }

        UpdateUI();
        state = GameState.None;
    }

    private Collapses Collapse(IEnumerable<int> columns) {
        Collapses collapses = new Collapses();
        foreach (int col in columns) {
            for (int row=0; row<rows-1; row++) {
                // Find removed pieces
                if (grid[row, col] == null) {
                    // Find next non-removed piece, move it down
                    for (int row2 = row+1; row2 < rows; row2++) {
                        if (grid[row2, col] != null) {
                            grid[row, col] = grid[row2, col];
                            grid[row2, col] = null;

                            grid[row, col].GetComponent<Piece>().row = row;
                            grid[row, col].GetComponent<Piece>().col = col;

                            if (row2 - row > collapses.maxDistance)
                                collapses.maxDistance = row2 - row;
                            collapses.AddObject(grid[row, col]);

                            break;
                        }
                    }
                }
            }
        }
        return collapses;
    }

    private void Swap(GameObject g1, GameObject g2) {
        Piece p1 = g1.GetComponent<Piece>();
        Piece p2 = g2.GetComponent<Piece>();

        // Swap in the grid
        GameObject tmp = grid[p1.row, p1.col];
        grid[p1.row, p1.col] = grid[p2.row, p2.col];
        grid[p2.row, p2.col] = tmp;

        // Update piece positions
        p1.SwapWith(p2);

        // Swap the rendered pieces
        g1.transform.positionTo(animationDuration, g2.transform.position);
        g2.transform.positionTo(animationDuration, g1.transform.position);
    }

    private void UpdateUI() {
        turnsBar.value = (float)turnsLeft/totalTurns;
    }


    private Matches GetMatches(GameObject go) {
        Matches matches = new Matches();
        var hMatches = HorizontalMatches(go);
        var vMatches = VerticalMatches(go);
        matches.AddObjects(hMatches);
        matches.AddObjects(vMatches);
        return matches;
    }

    private IEnumerable<GameObject> GetMatches(IEnumerable<GameObject> gos) {
        List<GameObject> matches = new List<GameObject>();
        foreach (GameObject go in gos) {
            matches.AddRange(GetMatches(go).matches);
        }
        return matches.Distinct();
    }

    private IEnumerable<GameObject> HorizontalMatches(GameObject go) {
        List<GameObject> matches = new List<GameObject>();
        Piece p = go.GetComponent<Piece>();
        matches.Add(go);

        if (p.col > 0) {
            for (int col=p.col-1; col>=0; col--) {
                if (grid[p.row, col].GetComponent<Piece>().Equal(p)) {
                    matches.Add(grid[p.row, col]);
                }
                else {
                    break;
                }
            }
        }
        if (p.col < cols-1) {
            for (int col=p.col+1; col<cols; col++) {
                if (grid[p.row, col].GetComponent<Piece>().Equal(p)) {
                    matches.Add(grid[p.row, col]);
                }
                else {
                    break;
                }
            }
        }

        if (matches.Count < minMatches) {
            matches.Clear();
        }
        return matches.Distinct();
    }

    private IEnumerable<GameObject> VerticalMatches(GameObject go) {
        List<GameObject> matches = new List<GameObject>();
        Piece p = go.GetComponent<Piece>();
        matches.Add(go);

        if (p.row > 0) {
            for (int row=p.row-1; row>=0; row--) {
                if (grid[row, p.col].GetComponent<Piece>().Equal(p)) {
                    matches.Add(grid[row, p.col]);
                }
                else {
                    break;
                }
            }
        }
        if (p.row < rows-1) {
            for (int row=p.row+1; row<rows; row++) {
                if (grid[row, p.col].GetComponent<Piece>().Equal(p)) {
                    matches.Add(grid[row, p.col]);
                }
                else {
                    break;
                }
            }
        }

        if (matches.Count < minMatches) {
            matches.Clear();
        }
        return matches.Distinct();
    }



    // Check if two pieces are valid neighbors,
    // i.e. not diagonal neighbors
    private bool ValidNeighbors(Piece p1, Piece p2) {
        return (p1.row == p2.row || p1.col == p2.col) && Math.Abs(p1.row - p2.row) <= 1 && Math.Abs(p1.col - p2.col) <= 1;
    }
}
