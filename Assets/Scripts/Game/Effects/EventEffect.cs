[System.Serializable]
public class EventEffect : IEffect {
    public GameEvent gameEvent;
    public float delay = 0;
    public float probability = 0;

    public override void Apply(Company company) {
        GameEvent ge = Instantiate(gameEvent) as GameEvent;
        gameEvent.delay = delay;
        gameEvent.probability = probability;

        GameManager.Instance.eventManager.Add(gameEvent);
    }

    public override void Remove(Company company) {
        // These can't be removed, or should they be?
    }
}
