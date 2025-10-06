using UnityEngine;

public class TriggerTeleport : MonoBehaviour
{
    [Tooltip("The place where things go....")]
    public Transform destination;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = destination.position;
    }
}
