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
    private float outragePenalty = -0.25f;
    private float happyBonus = 0.1f;

    private int rows;
    private int cols;
    public static float gridItemSize = 1f;
    public static float animationDuration = 0.2f;
    public static float moveAnimationDuration = 0.05f;

    public GameObject board;
    public GameObject[] productTypePrefabs;
    public GameObject[] influencerPrefabs;
    public GameObject outragePrefab;
    public GameObject happyPrefab;
    public GameObject bugPrefab;
    public GameObject blockPrefab;
    public GameObject emptyPrefab;
    public GameObject[,] grid;
    public float[,] bonusGrid;

    public Color productTypeColor;
    public Color influencerColor;
    public Color hazardColor;
    public Color emptyColor;
    public Color activeColor;
    public GameObject tilePrefab;
    public GameObject iconPrefab;
    public Sprite plusIcon;
    public Sprite minusIcon;

    public Camera camera;
    public MainGameUI ui;

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

        // At minimum, 10 turns
        //totalTurns = Math.Max(10, (int)Math.Floor(company.productivity/workUnit));
        // testing
        totalTurns = 30;
        turnsLeft = totalTurns;

        ui.gameObject.SetActive(true);
        camera.gameObject.SetActive(true);
        ui.Setup(company, camera, totalTurns);

        CreateNextPiece();
        InitGrid();
        UpdateUI();
    }

    void OnDisable() {
        ui.gameObject.SetActive(false);
        camera.gameObject.SetActive(false);
    }


    private void InitGrid() {
        // TODO these depend on number of locations owned or something
        rows = 5;
        cols = 5;
        grid = new GameObject[rows, cols];
        bonusGrid = new float[rows, cols];

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
                bonusGrid[r,c] = 0f;
            }
        }
    }

    private void SetBonusAt(int r, int c, float bonus) {
        bonusGrid[r,c] += bonus;
        GameObject icon = grid[r,c].transform.Find("Icon").gameObject;

        if (bonusGrid[r,c] < 0) {
            icon.GetComponent<SpriteRenderer>().sprite = minusIcon;
            icon.SetActive(true);
        } else if (bonusGrid[r,c] > 0) {
            icon.GetComponent<SpriteRenderer>().sprite = plusIcon;
            icon.SetActive(true);
        } else {
            icon.SetActive(false);
        }
    }

    private Color ColorForPiece(Piece p) {
        switch (p.type) {
            case Piece.Type.ProductType:
                return productTypeColor;
                break;
            case Piece.Type.Bug:
                return hazardColor;
                break;
            case Piece.Type.Influencer:
                return influencerColor;
                break;
            case Piece.Type.Empty:
                return emptyColor;
                break;
        }
        return emptyColor;
    }

    private GameObject PlacePiece(GameObject piece, int r, int c) {
        Vector2 pos = new Vector2(c * gridItemSize, r * gridItemSize);

        // Remove existing piece if necessary
        if (grid[r,c] != null)
            RemovePieceAt(r, c);

        piece.transform.position = startPos + pos;
        piece.GetComponent<Piece>().PlaceAt(r, c);
        piece.transform.parent = board.transform;
        grid[r, c] = piece;
        piece.SetActive(true);

        // Outrage/happy irradiates nearby tiles
        if (piece.GetComponent<Piece>().type == Piece.Type.Outrage) {
            SetBonusAround(r, c, outragePenalty);
        } else if (piece.GetComponent<Piece>().type == Piece.Type.Happy) {
            SetBonusAround(r, c, happyBonus);
        }

        return piece;
    }

    private void SetBonusAround(int r, int c, float bonus) {
        for (int col=c-1; col<=c+1; col++) {
            if (col < 0 || col > cols-1)
                continue;

            for (int row=r-1; row<=r+1; row++) {
                if (row < 0 || row > rows-1)
                    continue;
                SetBonusAt(row, col, bonus);
            }
        }
    }

    private void RemovePieceAt(int r, int c) {
        Destroy(grid[r, c]);
        grid[r, c] = null;
    }

    private GameObject CreatePiece(GameObject piecePrefab) {
        GameObject piece = Instantiate(piecePrefab, Vector2.zero, piecePrefab.transform.rotation) as GameObject;
        piece.name = piece.name.Replace("(Clone)", "");
        piece.SetActive(false);

        GameObject tile = Instantiate(tilePrefab) as GameObject;
        tile.GetComponent<SpriteRenderer>().color = ColorForPiece(piece.GetComponent<Piece>());
        tile.name = "Tile";
        tile.transform.parent = piece.transform;
        tile.transform.localPosition = new Vector3(0,0,2);

        GameObject icon = Instantiate(iconPrefab) as GameObject;
        icon.name = "Icon";
        icon.transform.parent = piece.transform;
        icon.transform.localPosition = new Vector3(0.2f,-0.25f,-1);
        icon.SetActive(false);

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
        nextPiece.SetActive(true);

        ui.ShowNextPiece(nextPiece);
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
                        case Piece.Type.Influencer:
                            PopInfluencer(p.name, p.gameObject.transform.position);
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
                        StartCoroutine(MergeProductTypes(hit.collider.gameObject, hitGo));
                    }
                }
            }
        }
    }

    private void UpdateUI() {
        ui.UpdateUI(turnsLeft, totalTurns);
    }

    private void TakeTurn() {
        turnsLeft--;

        if (turnsLeft <= 0 && Done != null) {
            Done();
        }

        UpdateUI();
    }

    private void PopInfluencer(string name, Vector2 pos) {
        int amount = 0;
        switch (name) {
            case "Friend":
                amount = 15;
                break;
            case "Journalist":
                amount = 35;
                break;
            case "Thought Leader":
                amount = 70;
                break;
        }
        company.goodwill += amount;
        ui.ShowResultAt(pos, string.Format("+{0}", amount));
    }

    private void ProcessMatchesAround(GameObject piece) {
        List<GameObject> hMatches = HorizontalMatches(piece).ToList();
        List<GameObject> vMatches = VerticalMatches(piece).ToList();

        // TODO animate the merging

        if (hMatches.Count > 0) {
            // Collapse horizontal matches leftwards
            ProcessMatchesAround(MergePieces(hMatches));
        }

        if (vMatches.Count > 0) {
            // Collapse vertical matches downwards
            ProcessMatchesAround(MergePieces(vMatches));
        }
    }

    private GameObject MergePieces(List<GameObject> pieces) {
        // Assumes newest piece is first
        GameObject merged = pieces[0];
        pieces.RemoveAt(0);
        foreach (GameObject p in pieces) {
            GameObject empty = CreatePiece(emptyPrefab);
            PlacePiece(empty, p.GetComponent<Piece>().row, p.GetComponent<Piece>().col);
        }
        Piece mPiece = merged.GetComponent<Piece>();
        switch (mPiece.type) {
            case Piece.Type.ProductType:
                merged.transform.Find("Tile").GetComponent<SpriteRenderer>().color = activeColor;
                mPiece.stacked = true;
                break;

            case Piece.Type.Influencer:
                switch (merged.name) {
                    case "Friend":
                        merged = PlacePiece(CreatePiece(influencerPrefabs[1]), mPiece.row, mPiece.col);
                        break;
                    case "Journalist":
                        merged = PlacePiece(CreatePiece(influencerPrefabs[2]), mPiece.row, mPiece.col);
                        break;
                    case "Thought Leader":
                        merged = PlacePiece(CreatePiece(happyPrefab), mPiece.row, mPiece.col);
                        break;
                }
                break;
        }
        return merged;
    }


    private IEnumerator MergeProductTypes(GameObject g1, GameObject g2) {
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
            Product product = company.LaunchProduct(new List<ProductType> {pt1, pt2}, 1+bonusGrid[p1.row, p1.col]);
            float revenue = product.revenue;

            ui.ShowResultAt((g1.transform.localPosition + g2.transform.localPosition)/2,
                    string.Format("{0:C0}", revenue));
            ui.ShowProductInfo(product);

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

        // Add last-placed item to beginning of list, so it's easily accessible
        matches.Insert(0, go);

        if (matches.Count < minMatches) {
            matches.Clear();
        }

        return matches.Distinct();
    }

    private IEnumerable<GameObject> VerticalMatches(GameObject go) {
        List<GameObject> matches = new List<GameObject>();
        Piece p = go.GetComponent<Piece>();

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

        // Add last-placed item to beginning of list, so it's easily accessible
        matches.Insert(0, go);

        if (matches.Count < minMatches) {
            matches.Clear();
        }

        return matches.Distinct();
    }


    // Check if two pieces are valid neighbors,
    // i.e. not diagonal neighbors and neither is empty
    private bool ValidNeighbors(Piece p1, Piece p2) {
        return p1.type != Piece.Type.Empty && p2.type != Piece.Type.Empty
            && (p1.row == p2.row || p1.col == p2.col)
            && Math.Abs(p1.row - p2.row) <= 1 && Math.Abs(p1.col - p2.col) <= 1;
    }
}
