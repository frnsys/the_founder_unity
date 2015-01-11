[System.Serializable]
public class CashEffect : IEffect {
    public float cash;

    public void Apply(Company company) {
        company.cash.baseValue += cash;
    }

    public void Remove(Company company) {
        // Cash effects don't have a reverse.
    }
}
