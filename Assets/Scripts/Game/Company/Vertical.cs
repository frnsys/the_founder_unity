using UnityEngine;

[System.Serializable]
public class Vertical : ScriptableObject, IHasPrereqs {
    public float cost = 10000000;

    public bool isAvailable(Company company) {
        // TO DO this should be the number of locations
        if (company.verticals.Count < company.workers.Count)
            return true;
        return false;
    }
}
