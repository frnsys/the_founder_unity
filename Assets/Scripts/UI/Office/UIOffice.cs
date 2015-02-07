using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public class UIOffice : MonoBehaviour {
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

    void OnEnable() {
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
