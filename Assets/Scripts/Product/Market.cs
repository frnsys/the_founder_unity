using UnityEngine;
using System.Collections;

public class Market : IUnlockable {
    public string name;

    public Market(string name_) {
        name = name_;
    }
}
