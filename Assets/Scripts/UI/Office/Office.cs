using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public class Office : MonoBehaviour {
    [System.Serializable]
    public struct PerkDisplay {
        public Perk perk;
        public PerkObject[] objects;
    }
    [System.Serializable]
    public struct PerkObject {
        public GameObject obj;
        public Vector3 position;
    }
    public PerkDisplay[] perkDisplays;

    // The bounds (x1,z1,x2,z2) of the office.
    public Vector4 bounds;

    // The UI objects (in-office buttons) and
    // the UI targets the they should follow.
    // These targets should match by index,
    // i.e. the i-th object should match with the i-th target.
    public GameObject[] uiObjects;
    public Transform[] uiTargets;

    // The cost for upgrading to this office.
    public float cost;

    // Employees required for this office.
    public int employeesRequired;

    // The next office after this one.
    public Office nextOffice;

    void OnEnable() {
        UIOfficeManager.Instance.SetupOfficeUI(uiObjects, uiTargets);
        Company.PerkBought += ShowPerk;
    }

    void OnDisable() {
        Company.PerkBought -= ShowPerk;
    }

    public void ShowPerk(Perk perk) {
        PerkDisplay pd = perkDisplays.FirstOrDefault(p => p.perk.name == perk.name);
        if (pd.perk != null) {
            // Get the current perk upgrade level.
            int upgradeLevel = perk.upgradeLevel;

            // It's possible that we don't have an object for higher upgrade levels,
            // so fall back to highest-level available object.
            int i = Math.Min(upgradeLevel, pd.objects.Length - 1);
            GameObject perkObj = Instantiate(pd.objects[i].obj) as GameObject;
            perkObj.transform.parent = transform;
            perkObj.transform.localPosition = pd.objects[i].position;
        }
    }
}
