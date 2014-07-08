using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameEffect {
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Industry> industries = new List<Industry>();
    public List<Market> markets = new List<Market>();
    public List<StatBuff> productBuffs = new List<StatBuff>();
    public List<StatBuff> workerBuffs = new List<StatBuff>();
    public List<StatBuff> companyBuffs = new List<StatBuff>();
}
