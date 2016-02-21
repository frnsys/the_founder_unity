using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public struct Position {
    public int col, row;
    public Position(int c, int r) {
        col = c;
        row = r;
    }
    public static Position operator +(Position p1, Position p2) {
      return new Position(p1.col + p2.col, p1.row + p2.row);
    }
}

public class Board : MonoBehaviour {
    public float tileWidth;
    public float tileHeight;

    private Tile[,] grid;
    private int maxCols;
    private int maxRows;
    private List<float> incomeDistribution;
    private float resourceProb = 0.1f;
    private float officeProb = 0.05f;
    private float emptyProb = 0.2f;
    private Vector2 startPos;

    public GameObject tilePrefab;
    public Sprite[] tileSprites;

    public void Setup(int n_tiles, int max_cols, int max_rows, List<float> income_dist, List<Player> players) {
        // Clear the grid
        foreach (Transform child in transform) {
             Destroy(child.gameObject);
        }

        if (n_tiles > max_cols * max_rows)
            throw new System.ArgumentException("Number of tiles must be less than the max cols * max rows");

        maxCols = max_cols;
        maxRows = max_rows;
        incomeDistribution = income_dist;
        grid = new Tile[max_rows, max_cols];

        GenerateBoard(n_tiles, players);
    }

    private List<Position> adjacentPositions = new List<Position> {
        new Position(0, -1), // upper left
        new Position(1, -1), // upper right
        new Position(-1, 0), // left
        new Position(1, 0),  // right
        new Position(-1, 1), // bottom left
        new Position(0, 1),  // bottom right
    };


    private List<Position> AdjacentPositions(Position p) {
        List<Position> positions = new List<Position>();

        foreach (Position pos in adjacentPositions) {
            Position adjPos = p + pos;

            // check that the position is still on the board
            if (adjPos.row > 0 && adjPos.row < maxRows &&
                    adjPos.col > 0 && adjPos.col < maxCols) {
                positions.Add(adjPos);
            }
        }
        return positions;
    }

    private List<Position> OpenPositions(List<Position> positions) {
        List<Position> openPositions = new List<Position>();
        foreach (Position p in positions) {
            foreach (Position p_ in AdjacentPositions(p)) {
                if (grid[p_.row, p_.col] == null) {
                    openPositions.Add(p_);
                }
            }
        }
        return openPositions;
    }

    private void GenerateBoard(int n_tiles, List<Player> players) {
        Vector2 bottomRight = Camera.main.ViewportToWorldPoint(new Vector2(1,0));
        Vector2 topLeft = Camera.main.ViewportToWorldPoint(new Vector2(0,1));

        float worldWidth = bottomRight.x - topLeft.x;
        float worldHeight = topLeft.y - bottomRight.y;
        float gridWidth = (maxCols-1) * tileWidth;
        float gridHeight = (maxRows-1) * tileHeight;

        startPos = new Vector2(-gridWidth/2, -gridHeight/2);

        Position center = new Position(maxCols/2, maxRows/2);
        PlaceTileAt(RandomTile(), center.row, center.col);

        List<Position> occupiedPositions = new List<Position> { center };
        while (occupiedPositions.Count < n_tiles) {
            List<Position> openPositions = OpenPositions(occupiedPositions);
            Position pos = openPositions[UnityEngine.Random.Range(0, openPositions.Count - 1)];
            PlaceTileAt(RandomTile(), pos.row, pos.col);
            occupiedPositions.Add(pos);
        }

        PlaceHQs(occupiedPositions, players);

        // TODO set starting ownership for players (or just AI?)
    }

    private Tile RandomTile() {
        GameObject go = Instantiate(tilePrefab) as GameObject;
        float roll = UnityEngine.Random.value;
        if (roll <= emptyProb) {
            go.GetComponent<SpriteRenderer>().sprite = tileSprites[0];
            Tile tile = go.AddComponent<Tile>() as Tile;
            return tile;
        } else if (roll <= emptyProb + resourceProb) {
            ResourceTile tile = go.AddComponent<ResourceTile>() as ResourceTile;
            tile.RandomizeType();
            Sprite sprite = tileSprites[6];
            switch (tile.type) {
                case ResourceTile.Type.Datacenter:
                    sprite = tileSprites[6];
                    break;
                case ResourceTile.Type.Factory:
                    sprite = tileSprites[7];
                    break;
                case ResourceTile.Type.Lab:
                    sprite = tileSprites[8];
                    break;
                default:
                    sprite = tileSprites[9];
                    break;
            }
            go.GetComponent<SpriteRenderer>().sprite = sprite;
            return tile;
        } else if (roll <= emptyProb + resourceProb + officeProb) {
            go.GetComponent<SpriteRenderer>().sprite = tileSprites[1];
            OfficeTile tile = go.AddComponent<OfficeTile>() as OfficeTile;
            return tile;
        } else {
            IncomeTile tile = go.AddComponent<IncomeTile>() as IncomeTile;
            tile.RandomizeIncome(incomeDistribution);
            Sprite sprite = tileSprites[2];
            switch (tile.income) {
                case 0:
                    sprite = tileSprites[2];
                    break;
                case 1:
                    sprite = tileSprites[3];
                    break;
                case 2:
                    sprite = tileSprites[4];
                    break;
                default:
                    sprite = tileSprites[5];
                    break;
            }
            go.GetComponent<SpriteRenderer>().sprite = sprite;
            return tile;
        }
    }

    private void PlaceHQs(List<Position> validPositions, List<Player> players) {
        List<Position> startingPositions = new List<Position>();

        // place first player at random location
        startingPositions.Add(validPositions[UnityEngine.Random.Range(0, validPositions.Count - 1)]);

        // place each other player as far as possible from the other players
        foreach (Player p in players.Skip(1)) {
            float bestScore = 0f;
            Position bestPos = new Position(maxCols/2, maxRows/2);
            // not efficient, but working at a small enough scale where it's ok
            foreach (Position pos in validPositions) {
                // score positions by the square sum of their manhattan distances to each existing starting position
                float score = (float)startingPositions.Select(x => Math.Pow(ManhattanDistance(pos, x), 2)).Sum();
                if (score > bestScore) {
                    bestScore = score;
                    bestPos = pos;
                }
            }
            startingPositions.Add(bestPos);
        }

        // place the HQ tiles
        for (int i=0; i < startingPositions.Count; i++) {
            OfficeTile hq = new OfficeTile();
            hq.owner = players[i];
            grid[startingPositions[i].row, startingPositions[i].col] = hq;
        }
    }

    private void SetStartingOwnership(List<Player> players) {
        foreach (Player p in players) {
            // skip human player
            if (p.type == Player.Type.Human)
                continue;

            int num_owned = (int)p.company.marketShare;
        }
        // TODO
    }

    private int ManhattanDistance(Position p1, Position p2) {
        // use manhattan distance as a heuristic
        return Math.Abs(p1.col - p2.col) + Math.Abs(p1.row - p2.row);
    }

    public void PlaceTileAt(Tile tile, int r, int c) {
        // Remove existing piece if necessary
        if (grid[r,c] != null)
            RemoveTileAt(r, c);

        Vector2 pos = new Vector2(c * tileWidth + ((r%2) * tileWidth/2), r * tileHeight);
        tile.transform.position = startPos + pos;
        tile.transform.parent = transform;
        grid[r, c] = tile;
        tile.gameObject.SetActive(true);
    }

    private void RemoveTileAt(int r, int c) {
        Destroy(grid[r, c].gameObject);
        grid[r, c] = null;
    }

    //private void SetBonusAround(int r, int c, float bonus) {
        //for (int col=c-1; col<=c+1; col++) {
            //if (col < 0 || col > cols-1)
                //continue;

            //for (int row=r-1; row<=r+1; row++) {
                //if (row < 0 || row > rows-1)
                    //continue;
                //SetBonusAt(row, col, bonus);
            //}
        //}
    //}

    //private void SetBonusAt(int r, int c, float bonus) {
        //bonusGrid[r,c] += bonus;
        //GameObject icon = grid[r,c].transform.Find("Icon").gameObject;

        //if (bonusGrid[r,c] < 0) {
            //icon.GetComponent<SpriteRenderer>().sprite = minusIcon;
            //icon.SetActive(true);
        //} else if (bonusGrid[r,c] > 0) {
            //icon.GetComponent<SpriteRenderer>().sprite = plusIcon;
            //icon.SetActive(true);
        //} else {
            //icon.SetActive(false);
        //}
    //}
}
