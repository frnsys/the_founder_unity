[System.Serializable]
public class OpinionEffect : IEffect {
    public OpinionEvent opinionEvent;

    public void Apply(Company company) {
        company.ApplyOpinionEvent(opinionEvent);
    }

    public void Remove(Company company) {
        // No removing of opinion events,
        // they degrade naturally via "forgetting".
    }
}
