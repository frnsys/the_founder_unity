using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MainGame : MonoBehaviour {
    static public event System.Action Done;

    // For the narrative manager
    static public event System.Action ProductCreated;
    static public event System.Action ProductFailed;
    static public event System.Action<Piece> PiecePlaced;
    static public event System.Action<Piece> PieceQueued;

    private int totalTurns;
    private int turnsLeft;

    private Company company;
    private EventManager em;

    private int workUnit = 10; // TODO balance this
    private int outrageCost = 50;
    private int minMatches = 2;
    private float outragePenalty = -0.25f;
    private float happyBonus = 0.1f;
    private int locationsPerRow = 5;

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
    public GameObject hypePrefab;
    public GameObject emptyPrefab;
    public GameObject[,] grid;
    public float[,] bonusGrid;
    private List<GameObject> validProductTypePrefabs;

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
    public UIMainGame ui;

    private GameObject hitGo;
    private Vector2 startPos;
    private GameObject nextPiece;
    private Piece selectedPiece;


    private enum GameState {
        None,
        Selecting,
        Animating
    }
    private GameState state;

    public void Setup(List<ProductType> productTypes) {
        validProductTypePrefabs = new List<GameObject>();
        foreach (ProductType pt in productTypes) {
            GameObject prefab = productTypePrefabs.Where(p => p.name == pt.name).First();
            validProductTypePrefabs.Add(prefab);
        }

        company = GameManager.Instance.playerCompany;
        em = GameManager.Instance.eventManager;
        state = GameState.None;

        // At minimum, 10 turns
        totalTurns = Math.Max(10, (int)Math.Floor(company.productivity/workUnit));
        turnsLeft = totalTurns;

        ui.gameObject.SetActive(true);
        camera.gameObject.SetActive(true);
        ui.Setup(company, camera, totalTurns);

        if (nextPiece != null)
            Destroy(nextPiece.gameObject);

        InitGrid();
        CreateNextPiece();
    }

    void OnEnable() {
        Promo.Completed += OnPromoCompleted;
    }

    void OnDisable() {
        if (ui != null)
            ui.gameObject.SetActive(false);
        if (camera != null)
            camera.gameObject.SetActive(false);
        Promo.Completed -= OnPromoCompleted;
    }


    void OnPromoCompleted(Promo p) {
        int hype = p.hype;
        company.hype += hype;
        ui.ShowInfo(string.Format("Ran {0} promo", p.name),
                string.Format("+{0} hype", hype));

        PlacePiece(CreatePiece(emptyPrefab), selectedPiece.row, selectedPiece.col);
        TakeTurn();
    }


    private void InitGrid() {
        // Clear the grid
        foreach (Transform child in transform) {
             Destroy(child.gameObject);
        }

        rows = 3 + (int)Math.Floor((float)company.locations.Count/locationsPerRow);
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
        if (UnityEngine.Random.value <= Mathf.Max(0.02f, -company.opinion/100)) {
            return outragePrefab;

        // TODO balance this
        } else if (UnityEngine.Random.value < Mathf.Min(0.15f, company.hype/100)) {
            float roll = UnityEngine.Random.value;
            if (roll <= 0.55f)
                return influencerPrefabs[0];
            else if (roll <= 0.9f)
                return influencerPrefabs[1];
            else
                return influencerPrefabs[2];
        } else if (UnityEngine.Random.value < 0.1f) {
            return hypePrefab;
        } else {
            return validProductTypePrefabs[UnityEngine.Random.Range(0, validProductTypePrefabs.Count)];
        }
    }

    private void CreateNextPiece() {
        nextPiece = CreatePiece(RandomPiecePrefab());
        nextPiece.SetActive(true);

        // For narrative
        if (PieceQueued != null)
            PieceQueued(nextPiece.GetComponent<Piece>());

        ui.ShowNextPiece(nextPiece);
    }

    void Update() {
        if (state == GameState.None && !UIManager.Instance.isDisplaying) {
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
                        // Placing a piece doesn't take a turn
                        case Piece.Type.Empty:
                            // For narrative
                            if (PiecePlaced != null)
                                PiecePlaced(nextPiece.GetComponent<Piece>());

                            PlacePiece(nextPiece, p.row, p.col);
                            ProcessMatchesAround(nextPiece);
                            CreateNextPiece();
                            break;
                        case Piece.Type.Outrage:
                            if (company.hype - outrageCost >= 0) {
                                company.hype -= outrageCost;
                                SetBonusAround(p.row, p.col, -outragePenalty);
                                PlacePiece(CreatePiece(emptyPrefab), p.row, p.col);
                                ui.ShowInfo("Defused outrage", string.Format("-{0} hype", outrageCost));
                                TakeTurn();
                            } else {
                                ui.ShowInfo("Not enough hype", string.Format("Needs {0} hype", outrageCost));
                            }
                            break;
                        case Piece.Type.Bug:
                            PlacePiece(CreatePiece(emptyPrefab), p.row, p.col);
                            ui.ShowInfo("Debugged", "");
                            TakeTurn();
                            break;
                        case Piece.Type.Influencer:
                            int amount = PopInfluencer(p.name, p.gameObject.transform.position);
                            PlacePiece(CreatePiece(emptyPrefab), p.row, p.col);
                            ui.ShowInfo(string.Format("Influenced {0}", p.name), string.Format("+{0} hype", amount));
                            TakeTurn();
                            break;
                        case Piece.Type.Hype:
                            selectedPiece = p;
                            UIManager.Instance.ShowPromos();
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

    private void TakeTurn() {
        selectedPiece = null;
        turnsLeft--;
        ui.TakeTurn();

        if (turnsLeft <= 0 && Done != null) {
            Done();
        } else {
            // Resolve events
            em.Tick();
            em.EvaluateSpecialEvents();
        }

        // Public forgetting
        company.Forget();
    }

    private int PopInfluencer(string name, Vector2 pos) {
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
        company.hype += amount;
        company.opinion += amount/2;
        ui.ShowResultAt(pos, string.Format("+{0}", amount));
        return amount;
    }

    private void ProcessMatchesAround(GameObject piece) {
        Piece.Type t = piece.GetComponent<Piece>().type;
        if (t != Piece.Type.Influencer && t != Piece.Type.ProductType)
            return;

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
        Piece p1 = g1.GetComponent<Piece>(); // to piece
        Piece p2 = g2.GetComponent<Piece>(); // from piece

        // Animate
        Vector2 oldPos = g2.transform.position;
        g2.transform.positionTo(animationDuration, g1.transform.position);
        yield return new WaitForSeconds(animationDuration);

        if (p1.stacked && p2.stacked) {
            // TODO a nice little flash of light or something
            ProductType pt1 = ProductType.Load(p1.name);
            ProductType pt2 = ProductType.Load(p2.name);
            Product product = company.LaunchProduct(new List<ProductType> {pt1, pt2}, 1+bonusGrid[p1.row, p1.col]);

            if (product == null) {
                // Failure - bug
                GameObject piece = CreatePiece(bugPrefab);
                PlacePiece(piece, p1.row, p1.col);
                ui.ShowResultAt(g1.transform.localPosition, "[c][FC3941]FAILURE[-][/c]");

                // For narrative
                if (ProductFailed != null)
                    ProductFailed();
            } else {
                float revenue = product.revenue;
                ui.ShowResultAt((g1.transform.localPosition + g2.transform.localPosition)/2,
                        string.Format("{0:C0}", revenue));
                ui.ShowProductInfo(product);
                GameObject e1 = CreatePiece(emptyPrefab);
                PlacePiece(e1, p1.row, p1.col);

                // For narrative
                if (ProductCreated != null)
                    ProductCreated();
            }

            GameObject e2 = CreatePiece(emptyPrefab);
            PlacePiece(e2, p2.row, p2.col);

            TakeTurn();

        } else if (p2.stacked && p1.type == Piece.Type.Empty) {
            grid[p2.row, p2.col] = null;
            GameObject e2 = CreatePiece(emptyPrefab);
            PlacePiece(e2, p2.row, p2.col);
            PlacePiece(g2, p1.row, p1.col);
            TakeTurn();

        } else {
            // Undo
            g2.transform.positionTo(animationDuration, oldPos);
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
    // i.e. not diagonal neighbors
    private bool ValidNeighbors(Piece p1, Piece p2) {
        return (p1.row == p2.row || p1.col == p2.col)
            && Math.Abs(p1.row - p2.row) <= 1 && Math.Abs(p1.col - p2.col) <= 1;
    }
}
