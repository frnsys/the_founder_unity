using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Matches {
    private List<GameObject> _matches;

    public Matches() {
        _matches = new List<GameObject>();
    }

    public IEnumerable<GameObject> matches {
        get { return _matches.Distinct(); }
    }

    public void AddObject(GameObject go) {
        if (!_matches.Contains(go))
            _matches.Add(go);
    }

    public void AddObjects(IEnumerable<GameObject> gos) {
        foreach (GameObject go in gos) {
            AddObject(go);
        }
    }
}
