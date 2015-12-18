using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {
    public int col;
    public int row;
    public GameObject obj;

    public enum Type {
        ProductType,
        Product,
        Influencer,
        Bug,
        Outrage,
        Happy,
        Empty
    }
    public Type type;

    public void PlaceAt(int row_, int col_) {
        col = col_;
        row = row_;
    }

    public bool Equal(Piece p) {
        if (p != null && type == p.type && name == p.name) {
            return true;
        }
        return false;
    }

    void Update() {
        if (obj != null)
            UIAnimator.Rotate(obj);
    }

    public void SwapWith(Piece p) {
        int tmp = p.col;
        p.col = col;
        col = tmp;

        tmp = p.row;
        p.row = row;
        row = tmp;
    }
}
