using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Consumer : MonoBehaviour {
    [HideInInspector]
    public Vector3 target;
    private NavMeshAgent agent;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        if (target != null)
            agent.SetDestination(target);
    }
}
