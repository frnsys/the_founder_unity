using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Collapses {
    private List<GameObject> _pieces;
    public int maxDistance;

    public Collapses() {
        _pieces = new List<GameObject>();
    }

    public IEnumerable<GameObject> pieces {
        get { return _pieces.Distinct(); }
    }

    public void AddObject(GameObject go) {
        if (!_pieces.Contains(go))
            _pieces.Add(go);
    }
}
