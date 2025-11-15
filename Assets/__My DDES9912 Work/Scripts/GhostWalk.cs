using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GhostWalk : MonoBehaviour
{

    public NavMeshAgent myAgent;
    public Transform destination;

     /* Header("Chase Settings")] */

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        myAgent.destination = destination.position;
    }
}
