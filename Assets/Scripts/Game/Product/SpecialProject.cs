using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SpecialProject : TemplateResource<SpecialProject> {
    public string description;
    public float cost;
    public EffectSet effects = new EffectSet();
    public ProductRecipe[] requiredProducts;
    public Mesh mesh;

    public override string ToString() {
        return name;
    }

    static public event System.Action<SpecialProject> Completed;
    public void Develop() {
        if (Completed != null)
            Completed(this);
    }

    public bool isAvailable(Company company) {
        if (!(company.cash.value >= cost))
            return false;

        return requiredProducts.Length == company.products.Where(p => requiredProducts.Contains(p.Recipe)).Distinct().Count();
    }
}


