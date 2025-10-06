using UnityEngine;

public class PachinkoLauncher : MonoBehaviour 
{
    public Rigidbody subject;
    public Transform launchPoint;
    public float launchForce;
    public float launchForceDelta;

    private void Update()
    {
        launchForce = launchForce + launchForceDelta * Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        //asssing the next marble to be launched
        if(subject == null)
            subject = other.GetComponent<Rigidbody>();
    }

    public void Launch()
    {
        if(subject != null)//check to see if subject exists (there is a subject)
        {
            //move the marble to its launch position...
            subject.transform.position = launchPoint.position;

            //make sure the marble is stopped
            subject.linearVelocity = Vector3.zero;

            //launch the marble
            subject.AddForce(launchPoint.forward * launchForce);

            //make sure we never touch that marble again..
            subject = null;
        }
    }

    public void SetLaunchForce(float newForce)
    {
        launchForce = newForce;
    }

    public void SetLaunchForceDelta(float newAccumulate)
    {
        launchForceDelta = newAccumulate;
    }

    public void Reset()
    {
        launchForce = 0;
        launchForceDelta = 0;
    }
}
