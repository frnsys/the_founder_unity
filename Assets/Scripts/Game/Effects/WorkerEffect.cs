[System.Serializable]
public class WorkerEffect : IEffect {
    public StatBuff buff;

    public override void Apply(Company company) {
        foreach (Worker worker in company.workers) {
            worker.ApplyBuff(buff);
        }
    }

    public override void Remove(Company company) {
        foreach (Worker worker in company.workers) {
            worker.RemoveBuff(buff);
        }
    }

    public void Apply(Worker worker) {
        worker.ApplyBuff(buff);
    }
    public void Remove(Worker worker) {
        worker.RemoveBuff(buff);
    }
}
