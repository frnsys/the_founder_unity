[System.Serializable]
public class WorkerEffect : IEffect {
    public StatBuff buff;

    public void Apply(Company company) {
        foreach (Worker worker in company.workers) {
            worker.ApplyBuff(buff);
        }
    }

    public void Remove(Company company) {
        foreach (Worker worker in company.workers) {
            worker.RemoveBuff(buff);
        }
    }
}
