using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameEffect {
}

[System.Serializable]
public class ProductEffect : GameEffect {
    public List<ProductType> productTypes = new List<ProductType>();
    public List<Industry> industries = new List<Industry>();
    public List<Market> markets = new List<Market>();
    public List<StatBuff> buffs = new List<StatBuff>();
}

[System.Serializable]
public class WorkerEffect : GameEffect {
    public List<StatBuff> buffs = new List<StatBuff>();
}

[System.Serializable]
public class CompanyEffect : GameEffect {
    public List<StatBuff> buffs = new List<StatBuff>();
}

[System.Serializable]
public class EventEffect : GameEffect {
    public List<StatBuff> buffs = new List<StatBuff>();
}

[System.Serializable]
public class EconomyEffect : GameEffect {
    public List<StatBuff> buffs = new List<StatBuff>();
}

[System.Serializable]
public class UnlockEffect : GameEffect {
}







