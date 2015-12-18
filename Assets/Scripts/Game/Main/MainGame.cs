using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MainGame : MonoBehaviour {
    // Much credit due to github.com/dgkanatsios/matchthreegame for guidance
    static public event System.Action Done;

    private int totalTurns;
    private int turnsLeft;

    private Company company;
    private int workUnit = 10; // TODO balance this
    private int outrageCost = 50;
    private int minMatches = 3;

    private int rows;
    private int cols;
    private float gridItemSize = 1f;
    private float animationDuration = 0.2f;
    private float moveAnimationDuration = 0.05f;

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
    public Color activeColor;
    public GameObject tilePrefab;

    public GameObject ui;
    public Camera camera;
    public GameObject resultPrefab;
    public UIProgressBar turnsBar;
    public UILabel goodwillLabel;

    private GameObject hitGo;
    private Vector2 startPos;
    private GameObject nextPiece;

    private enum GameState {
        None,
        Selecting,
        Animating
    }
    private GameState state;

    void OnEnable() {
        company = GameManager.Instance.playerCompany;
        state = GameState.None;

        ui.SetActive(true);
        camera.gameObject.SetActive(true);

        // At minimum, 10 turns
        //totalTurns = Math.Max(10, (int)Math.Floor(company.productivity/workUnit));
        // testing
        totalTurns = 10;
        turnsLeft = totalTurns;

        CreateNextPiece();

        InitGrid();
        UpdateUI();
    }

    void OnDisable() {
        ui.SetActive(false);
        camera.gameObject.SetActive(false);
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
                GameObject piece = CreatePiece(emptyPrefab);
                PlacePiece(piece, r, c);
            }
        }
    }

    private void PlacePiece(GameObject piece, int r, int c) {
        Vector2 pos = new Vector2(c * gridItemSize, r * gridItemSize);

        // Remove existing piece if necessary
        if (grid[r,c] != null)
            RemovePieceAt(r, c);

        piece.transform.position = startPos + pos;
        piece.GetComponent<Piece>().PlaceAt(r, c);
        piece.transform.parent = board.transform;
        grid[r, c] = piece;
        piece.SetActive(true);
    }

    private void RemovePieceAt(int r, int c) {
        Destroy(grid[r, c]);
        grid[r, c] = null;
    }

    private GameObject CreatePiece(GameObject piecePrefab) {
        GameObject piece = Instantiate(piecePrefab, Vector2.zero, piecePrefab.transform.rotation) as GameObject;
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
            case Piece.Type.Empty:
                tile.GetComponent<SpriteRenderer>().color = emptyColor;
                break;
        }
        piece.name = piece.name.Replace("(Clone)", "");
        tile.name = tile.name.Replace("(Clone)", "");
        tile.transform.parent = piece.transform;
        tile.transform.localPosition = new Vector3(0,0,2);

        piece.SetActive(false);
        return piece;
    }

    private GameObject RandomPiecePrefab() {
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
            return productTypePrefabs[UnityEngine.Random.Range(0, 3)];
        }
    }

    private void CreateNextPiece() {
        nextPiece = CreatePiece(RandomPiecePrefab());
        Debug.Log(nextPiece.name);
    }

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
            // Double-click/tap
            if (Input.GetMouseButtonDown(0)) {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hitGo == hit.collider.gameObject) {
                    Piece p = hitGo.GetComponent<Piece>();
                    switch (p.type) {
                        case Piece.Type.Empty:
                            PlacePiece(nextPiece, p.row, p.col);
                            ProcessMatchesAround(nextPiece);
                            CreateNextPiece();
                            TakeTurn();
                            break;
                        case Piece.Type.Outrage:
                            if (company.goodwill - outrageCost >= 0) {
                                company.goodwill -= outrageCost;
                                PlacePiece(CreatePiece(emptyPrefab), p.row, p.col);
                                TakeTurn();
                            }
                            break;
                        case Piece.Type.Bug:
                            PlacePiece(CreatePiece(emptyPrefab), p.row, p.col);
                            TakeTurn();
                            break;
                    }
                    state = GameState.None;
                    return;
                }
            }
            // Swipe
            if (Input.GetMouseButton(0)) {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hitGo != hit.collider.gameObject) {
                    if (!ValidNeighbors(hit.collider.gameObject.GetComponent<Piece>(),
                            hitGo.GetComponent<Piece>())) {
                        state = GameState.None;
                    } else {
                        state = GameState.Animating;
                        StartCoroutine(MergePieces(hit.collider.gameObject, hitGo));
                    }
                }
            }
        }
    }

    private void UpdateUI() {
        turnsBar.value = (float)turnsLeft/totalTurns;
        goodwillLabel.text = string.Format("{0} goodwill", company.goodwill);
    }

    private void TakeTurn() {
        turnsLeft--;

        if (turnsLeft <= 0 && Done != null) {
            Done();
        }

        UpdateUI();
    }

    private void ProcessMatchesAround(GameObject piece) {
        List<GameObject> hMatches = HorizontalMatches(piece).ToList();
        List<GameObject> vMatches = VerticalMatches(piece).ToList();

        // TODO animate the merging

        if (hMatches.Count > 0) {
            // Collapse horizontal matches leftwards,
            // so keep the first horizontal match (assuming sorted)
            GameObject merged = hMatches[0];
            merged.transform.Find("Tile").GetComponent<SpriteRenderer>().color = activeColor;
            merged.GetComponent<Piece>().stacked = true;
            hMatches.RemoveAt(0);
            foreach (GameObject p in hMatches) {
                GameObject empty = CreatePiece(emptyPrefab);
                PlacePiece(empty, p.GetComponent<Piece>().row, p.GetComponent<Piece>().col);
            }
        }

        if (vMatches.Count > 0) {
            // Collapse vertical matches downwards,
            // so keep the first vertical match (assuming sorted)
            GameObject merged = vMatches[0];
            merged.transform.Find("Tile").GetComponent<SpriteRenderer>().color = activeColor;
            merged.GetComponent<Piece>().stacked = true;
            vMatches.RemoveAt(0);
            foreach (GameObject p in vMatches) {
                GameObject empty = CreatePiece(emptyPrefab);
                PlacePiece(empty, p.GetComponent<Piece>().row, p.GetComponent<Piece>().col);
            }
        }
    }


    private IEnumerator MergePieces(GameObject g1, GameObject g2) {
        Piece p1 = g1.GetComponent<Piece>();
        Piece p2 = g2.GetComponent<Piece>();

        // Animate
        Vector2 oldPos = g2.transform.position;
        g2.transform.positionTo(animationDuration, g1.transform.position);
        yield return new WaitForSeconds(animationDuration);

        if (!p1.stacked || !p2.stacked) {
            // Undo
            g2.transform.positionTo(animationDuration, oldPos);
        } else {
            // TODO a nice little flash of light or something
            ProductType pt1 = ProductType.Load(p1.name);
            ProductType pt2 = ProductType.Load(p2.name);
            float revenue = company.LaunchProduct(new List<ProductType> {pt1, pt2});

            ShowResultAt((g1.transform.localPosition + g2.transform.localPosition)/2,
                    string.Format("{0:C0}", revenue));

            GameObject e1 = CreatePiece(emptyPrefab);
            GameObject e2 = CreatePiece(emptyPrefab);
            PlacePiece(e1, p1.row, p1.col);
            PlacePiece(e2, p2.row, p2.col);

            TakeTurn();
        }

        state = GameState.None;
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
        return matches.Distinct().OrderBy(m => m.GetComponent<Piece>().col);
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
        return matches.Distinct().OrderBy(m => m.GetComponent<Piece>().row);
    }


    // Check if two pieces are valid neighbors,
    // i.e. not diagonal neighbors and neither is empty
    private bool ValidNeighbors(Piece p1, Piece p2) {
        return p1.type != Piece.Type.Empty && p2.type != Piece.Type.Empty
            && (p1.row == p2.row || p1.col == p2.col)
            && Math.Abs(p1.row - p2.row) <= 1 && Math.Abs(p1.col - p2.col) <= 1;
    }
}
