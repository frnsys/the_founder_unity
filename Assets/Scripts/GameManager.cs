using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class GameManager : MonoBehaviour {
    private Company playerCompany;
    private List<ProductType> unlockedProductTypes = new List<ProductType>();
    private List<Industry> unlockedIndustries = new List<Industry>();
    private List<Market> unlockedMarkets = new List<Market>();
    private List<GameEvent> gameEvents = new List<GameEvent>();

    // A list of events which could possibly occur.
    private List<GameEvent> candidateEvents = new List<GameEvent>();

    void Start() {
        LoadResources();
        //StartCoroutine(PayYourDebts());

        //Debug.Log(gameEvents.Count);
        //Debug.Log(System.Guid.NewGuid());
    }

    void Update() {
    }

    IEnumerator PayYourDebts() {
        while(true) {
            playerCompany.Pay();
            yield return new WaitForSeconds(60);
        }
    }

    public void LoadResources() {
        List<GameEvent> gameEvents = new List<GameEvent>(Resources.LoadAll<GameEvent>("GameEvents"));
    }


    void EnableEvent(GameEvent gameEvent) {
        // Add to candidates.
        candidateEvents.Add(gameEvent);

        // Subscribe to its effect events.
        gameEvent.EffectEvent += OnEffect;
    }

    void DisableEvent(GameEvent gameEvent) {
        if (candidateEvents.Contains(gameEvent)) {
            // Unsubscribe and remove.
            gameEvent.EffectEvent -= OnEffect;
            candidateEvents.Remove(gameEvent);
        }
    }

    void OnEffect(GameEffect effect) {
        switch(effect.type) {
            case GameEffect.Type.CASH:
                playerCompany.cash += effect.amount;
                break;

            case GameEffect.Type.ECONOMY:
                // TO DO
                break;

            case GameEffect.Type.PRODUCT:
                // TO DO
                break;

            case GameEffect.Type.WORKER:
                StatBuff buff = new StatBuff(effect.stat, effect.amount);
                foreach (Worker worker in playerCompany.workers) {
                    worker.ApplyBuff(buff);
                }
                break;

            case GameEffect.Type.EVENT:
                // TO DO
                break;

            case GameEffect.Type.UNLOCK:
                // TO DO
                break;
        }
    }
}


