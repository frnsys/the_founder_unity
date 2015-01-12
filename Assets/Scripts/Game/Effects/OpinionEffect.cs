[System.Serializable]
public class OpinionEffect : IEffect {
    public OpinionEvent opinionEvent;

    public override void Apply(Company company) {
        company.ApplyOpinionEvent(opinionEvent);
    }

    public override void Remove(Company company) {
        // No removing of opinion events,
        // they degrade naturally via "forgetting".
    }
}
