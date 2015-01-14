[System.Serializable]
public class PublicityCondition : Condition {
    public float value;

    public override bool Evaluate(Company company) {
        return company.publicity.value > value;
    }
}
