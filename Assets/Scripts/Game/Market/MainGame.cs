using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MainGame : MonoBehaviour {
    static public event System.Action Done;

    // For the narrative manager
    // TODO these may need to be replaced/tweaked/etc for the new strategic game
    static public event System.Action ProductCreated;
    static public event System.Action ProductFailed;

    public static float animationDuration = 0.2f;
    public static float moveAnimationDuration = 0.05f;

    private int totalTurns = 52; // 1 turn = 1 week
    private int turnsLeft;

    private Company company;
    private EventManager em;
    private List<Player> players;

    public Board board;
    public GameObject[] productTypePrefabs;
    private List<GameObject> validProductTypePrefabs;

    public Camera camera;
    public UIMainGame ui;

    private GameObject hitGo;
    private Piece selectedPiece;

    private enum GameState {
        None,
        Selecting,
        Animating
    }
    private GameState state;

    public void Setup(List<ProductType> productTypes, AAICompany opponent) {
        //validProductTypePrefabs = new List<GameObject>();
        //foreach (ProductType pt in productTypes) {
            //GameObject prefab = productTypePrefabs.Where(p => p.name == pt.name).First();
            //validProductTypePrefabs.Add(prefab);
        //}
        //
        company = GameManager.Instance.playerCompany;

        // create players
        players = new List<Player>() {
            new Player(company),
            new Player(opponent)
        };

        em = GameManager.Instance.eventManager;
        state = GameState.None;
        turnsLeft = totalTurns;

        // setup UI
        ui.gameObject.SetActive(true);
        camera.gameObject.SetActive(true);
        ui.Setup(company, camera, totalTurns);

        int n_tiles = 10; // TEMP, this should be determined by other factors
        List<float> income_dist = new List<float> { 0.65f, 0.20f, 0.125f, 0.025f }; // TEMP, this should be detemrined by other factors
        board.Setup(n_tiles, 5, 5, income_dist, players);
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

        //PlacePiece(CreatePiece(emptyPrefab), selectedPiece.row, selectedPiece.col);
        TakeTurn();
    }

    void Update() {
        //if (state == GameState.None && !UIManager.Instance.isDisplaying) {
            //// Click/tap
            //if (Input.GetMouseButtonDown(0)) {
                //var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                //if (hit.collider != null) {
                    //hitGo = hit.collider.gameObject;
                    //state = GameState.Selecting;
                //}
            //}
        //} else if (state == GameState.Selecting) {
            //// Double-click/tap
            //if (Input.GetMouseButtonDown(0)) {
                //var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                //if (hit.collider != null && hitGo == hit.collider.gameObject) {
                    //Piece p = hitGo.GetComponent<Piece>();
                    //switch (p.type) {
                        //// Placing a piece doesn't take a turn
                        //case Piece.Type.Empty:
                            //// For narrative
                            //if (PiecePlaced != null)
                                //PiecePlaced(nextPiece.GetComponent<Piece>());

                            //PlacePiece(nextPiece, p.row, p.col);
                            //ProcessMatchesAround(nextPiece);
                            //ui.ShowInfo(string.Format("Placed {0}", nextPiece.name), "");

                            //CreateNextPiece();
                            //break;
                        //case Piece.Type.Outrage:
                            //if (company.hype - outrageCost >= 0) {
                                //company.hype -= outrageCost;
                                //SetBonusAround(p.row, p.col, -outragePenalty);
                                //PlacePiece(CreatePiece(emptyPrefab), p.row, p.col);
                                //ui.ShowInfo("Defused outrage", string.Format("-{0} hype", outrageCost));
                                //TakeTurn();
                            //} else {
                                //ui.ShowInfo("Not enough hype", string.Format("Needs {0} hype", outrageCost));
                            //}
                            //break;
                        //case Piece.Type.Bug:
                            //PlacePiece(CreatePiece(emptyPrefab), p.row, p.col);
                            //ui.ShowInfo("Debugged", "");
                            //TakeTurn();
                            //break;
                        //case Piece.Type.Influencer:
                            //int amount = PopInfluencer(p.name, p.gameObject.transform.position);
                            //PlacePiece(CreatePiece(emptyPrefab), p.row, p.col);
                            //ui.ShowInfo(string.Format("Influenced {0}", p.name), string.Format("+{0} hype", amount));
                            //TakeTurn();
                            //break;
                        //case Piece.Type.Hype:
                            //selectedPiece = p;
                            //UIManager.Instance.ShowPromos();
                            //break;
                    //}
                    //state = GameState.None;
                    //return;
                //}
            //}
            //// Swipe
            //if (Input.GetMouseButton(0)) {
                //var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                //if (hit.collider != null && hitGo != hit.collider.gameObject) {
                    //if (!ValidNeighbors(hit.collider.gameObject.GetComponent<Piece>(),
                            //hitGo.GetComponent<Piece>())) {
                        //state = GameState.None;
                    //} else {
                        //state = GameState.Animating;
                        //StartCoroutine(MergeProductTypes(hit.collider.gameObject, hitGo));
                    //}
                //}
            //}
        //}
    }

    private void TakeTurn() {
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
        //company.Forget(); // TODO does this still need to be here?
    }

    private void HarvestTiles(Player p) {
        //float revenue = p.tiles.Select(t => t.income * 1000).Sum();
        // TODO add to player's cash
    }

    //private GameObject CreatePiece(GameObject piecePrefab) {
        //GameObject piece = Instantiate(piecePrefab, Vector2.zero, piecePrefab.transform.rotation) as GameObject;
        //piece.name = piece.name.Replace("(Clone)", "");
        //piece.SetActive(false);

        //GameObject tile = Instantiate(tilePrefab) as GameObject;
        //tile.GetComponent<SpriteRenderer>().color = ColorForPiece(piece.GetComponent<Piece>());
        //tile.name = "Tile";
        //tile.transform.parent = piece.transform;
        //tile.transform.localPosition = new Vector3(0,0,2);

        //GameObject icon = Instantiate(iconPrefab) as GameObject;
        //icon.name = "Icon";
        //icon.transform.parent = piece.transform;
        //icon.transform.localPosition = new Vector3(0.2f,-0.25f,-1);
        //icon.SetActive(false);

        //return piece;
    //}
}
