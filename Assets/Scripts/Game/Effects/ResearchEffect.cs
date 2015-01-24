[System.Serializable]
public class ResearchEffect : IEffect {
    public StatBuff buff = new StatBuff("Research", 0);

    public override void Apply(Company company) {
        company.research.ApplyBuff(buff);
    }

    public override void Remove(Company company) {
        company.research.RemoveBuff(buff);
    }
}
