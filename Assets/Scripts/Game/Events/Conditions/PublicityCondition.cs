[System.Serializable]
public class PublicityCondition : Condition {
    public float value;

    public virtual bool Evaluate(Company company) {
        return company.publicity.value > value;
    }
}
