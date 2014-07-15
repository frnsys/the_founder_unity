using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ProductEffect {
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Industry> industries = new List<Industry>();
    public List<Market> markets = new List<Market>();
    public List<StatBuff> buffs = new List<StatBuff>();
}
