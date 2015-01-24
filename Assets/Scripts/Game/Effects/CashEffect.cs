[System.Serializable]
public class CashEffect : IEffect {
    public float cash;

    public override void Apply(Company company) {
        company.cash.baseValue += cash;
    }

    public override void Remove(Company company) {
        // Cash effects don't have a reverse.
    }
}
