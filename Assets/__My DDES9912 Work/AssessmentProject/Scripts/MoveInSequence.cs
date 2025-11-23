using System.Collections;
using TMPro;
using UnityEngine;

public class MoveInSequence : MonoBehaviour
{
    //public Transform target;      // Assign in Inspector
    public float moveSpeed = 2f;  // Movement speed


    public InteractableGeneral subject;
    public GameObject targetGameObject; // Assign your target GameObject in the Inspector
    //private Vector3 currentPos;
    //public float speed; // Adjust speed as needed

    // We only want the tip of the pusher to collide - we don't want the pusher to go the the pivot point which is half way
    public float subtractXAmount = 1.15f; // The value to subtract from the X position
    public float subtractYAmount = 0.65f; // The value to subtract from the X position

    public float bounceForce = 10f; // Adjust this value to control bounce strength

    private bool arrived = false;

    private float startingX;
    private float startingY;
    private float startingZ;


    private void Start()
    {
        Vector3 currentPos = gameObject.transform.position;
        startingX = currentPos.x;
        startingY = currentPos.y;
        startingZ = currentPos.z;

        StartCoroutine(MoveToXZY());

    }

    private void Update()
    {
        // Now go back to starting position
        StartCoroutine(MoveToYZX());
    }

    // Move towards the target
    IEnumerator MoveToXZY()
    {
        // Get target position
        //Vector3 targetPos = target.position;
        Vector3 targetPos = targetGameObject.transform.position;

        // Subtract from the X & Y components
        targetPos.x -= subtractXAmount;
        targetPos.y -= subtractYAmount;

        // --- Step 1: Move to X ---
        yield return StartCoroutine(MoveToPosition(
            new Vector3(targetPos.x, transform.position.y, transform.position.z)));

        // --- Step 2: Move to Z ---
        yield return StartCoroutine(MoveToPosition(
            new Vector3(targetPos.x, transform.position.y, targetPos.z)));

        // --- Step 3: Move to Y ---
        yield return StartCoroutine(MoveToPosition(
            new Vector3(targetPos.x, targetPos.y, targetPos.z)));

        arrived = true;
    }

    // Move back to the starting point
    IEnumerator MoveToYZX()
    {

        // --- Step 1: Move to Y ---
        yield return StartCoroutine(MoveToPosition(
            new Vector3(transform.position.x, startingY, transform.position.z)));

        // --- Step 2: Move to Z ---
        yield return StartCoroutine(MoveToPosition(
            new Vector3(transform.position.x, startingY, startingZ)));

        // --- Step 3: Move to X ---
        yield return StartCoroutine(MoveToPosition(
            new Vector3(startingX, startingY, startingZ)));

        arrived = false;

    }

    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime);

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        subject = other.GetComponent<InteractableGeneral>();
        Debug.Log("OnTriggerEnter begin, and arrived = " + arrived);

        // Check if the entering object has a Rigidbody
        Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();     
   
        // Now bounce back
        if (subject != null)
        {

            // Invoke the interaction events (i.e. press the button & make the sound)
            subject.onPrimaryInteract.Invoke();

            Debug.Log("OnTriggerEnter invoke complete");

            if (arrived)
            {
                Debug.Log("We have arrived, so now try to return");
                // Now go back to starting position
                //StartCoroutine(MoveToYZX());
            }

        }
        
    }

}
