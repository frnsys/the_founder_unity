using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SpecialProject : SharedResource<SpecialProject> {
    public string description;
    public float cost;
    public EffectSet effects = new EffectSet();
    public List<ProductRecipe> requiredProducts;
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

        // TODO we either need to keep track of the product recipes the company has created or check this in some other way
        //return requiredProducts.Count == company.products.Where(p => requiredProducts.Contains(p.Recipe)).Distinct().Count();
        return true;
    }

    public static new SpecialProject Load(string name) {
        return Resources.Load("SpecialProjects/" + name) as SpecialProject;
    }
}


