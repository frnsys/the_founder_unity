using UnityEngine;
using System.Collections;

public class ProductType : IUnlockable {
    public string name;

    public ProductType(string name_) {
        name = name_;
    }
}
