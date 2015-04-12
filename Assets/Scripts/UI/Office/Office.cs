using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Office : MonoBehaviour {
    [System.Serializable]
    public struct PerkDisplay {
        public Perk perk;
        public GameObject[] objects;
    }
    public PerkDisplay[] perkDisplays;

    public Transform[] validEmployeeTargets;

    public enum Type {
        Apartment,
        Office,
        Campus
    }
    public Type type;

    // The bounds (x1,z1,x2,z2) of the office.
    // This should be in world coordinates.
    public Vector4 bounds;

    // The bounds (x1,y1,x2,y2) for the office camera.
    // This should be in world coordinates and should
    // be set at the office's minimimum orthographic size
    // and at a 16/9 (h/w) aspect ratio.
    public Vector4 cameraBounds;

    // The orthographic size limits of the office camera.
    public Vector2 cameraSizeLimits;

    // Where the office upgrade button should be positioned.
    public Vector3 upgradeButtonPosition;

    // The cost for upgrading to this office.
    public float cost;

    // Employees required for this office.
    public int employeesRequired;

    // The next office after this one.
    public Office nextOffice;

    void OnEnable() {
        Company.PerkBought += ShowPerk;
    }

    void OnDisable() {
        Company.PerkBought -= ShowPerk;
    }

    public void ShowPerk(APerk perk) {
        PerkDisplay pd = perkDisplays.FirstOrDefault(p => p.perk.name == perk.name);
        if (pd.perk != null) {
            // Get the current perk upgrade level.
            int upgradeLevel = perk.upgradeLevel;

            // It's possible that we don't have an object for higher upgrade levels,
            // so fall back to highest-level available object.
            int i = Math.Min(upgradeLevel, pd.objects.Length - 1);
            pd.objects[i].SetActive(true);
        }
    }
}
