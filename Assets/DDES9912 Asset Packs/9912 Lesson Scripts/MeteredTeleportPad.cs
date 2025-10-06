using UnityEngine;

public class MeteredTeleportPad : MonoBehaviour
{
    [Tooltip("Spawn point for the prize tray.")]
    public Transform destination;    
    [Tooltip("Usually your points counter")]
    public NumberHolder teleportCount;
    [Tooltip("Speed for 'Claim All' function")]
    public float emptyInterval = 1;

    [Header("System Settings - Usually Don't Touch")]
    public Rigidbody subject;
    public float teleportCost = 1;

    public void Teleport()
    {
        if (teleportCount.value > 0)
        {//Only give the user new balls if they have enough points.
            if (subject != null)
            {
                subject.transform.position = destination.position;                
                
                //Take away points as we give balls back...
                teleportCount.Subtract(teleportCost);

                //Prepare for the next ball...
                subject = null;
            }
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if(subject == null)
        {//Don't update the subject unless we've already got rid of the last one
            subject = other.GetComponent<Rigidbody>();
        }
    }

    public void ClaimAll()
    {
        if (teleportCount.value > 0)
        {
            Teleport();

            //Keep claiming my balls until I've run out of points
            Invoke("ClaimAll", emptyInterval);
        }
    }
}
