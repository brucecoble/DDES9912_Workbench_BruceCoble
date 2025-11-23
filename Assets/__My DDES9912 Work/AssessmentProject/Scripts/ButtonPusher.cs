using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;


public class ButtonPusher : MonoBehaviour
{
    public InteractableGeneral subject;
    public GameObject targetGameObject; // Assign your target GameObject in the Inspector
    private Vector3 currentPos;
    private Vector3 origin;
    public float speed; // Adjust speed as needed

    // We only want the tip of the pusher to collide - we don't want the pusher to go the the pivot point which is half way
    public float subtractXAmount = 1.15f; // The value to subtract from the X position
    public float subtractYAmount = 0.65f; // The value to subtract from the X position

    public float bounceForce = 10f; // Adjust this value to control bounce strength

    void Awake()
    {
        origin = transform.position;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get this object's position
        currentPos = transform.position;

        if (targetGameObject != null)
        {
            // Get the target GameObject's position
            Vector3 targetPosition = targetGameObject.transform.position;

            // Subtract from the X & Y components
            targetPosition.x -= subtractXAmount;
            targetPosition.y -= subtractYAmount;

            // Get this object's position
            Vector3 currentPos = transform.position;
            Debug.Log("Current Position: " + currentPos);

            // Move the current GameObject to the target's position
            // This will instantly teleport the object
            //transform.position = targetPosition;

            // To move smoothly, you can use Vector3.Lerp or Vector3.MoveTowards
            // Example using Lerp for smooth movement:
            // float speed = 5f; // Adjust speed as needed
            // transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);

            // Example using MoveTowards for consistent speed movement:
            //float speed = 5f; // Adjust speed as needed
            // x = -1.15
            // y = -0.65
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
        }
        else
        {
            Debug.LogWarning("Target GameObject not assigned to MoveToTarget script on " + gameObject.name);
        }

    }

    // Update is called once per frame
    void Update()
    {
        

        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        subject = other.GetComponent<InteractableGeneral>();
        Debug.Log("OnTriggerEnter begin");

        // Check if the entering object has a Rigidbody
        Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();

        // Now bounce back
        if (subject != null)
        {

            // Invoke the interaction events (i.e. press the button & make the sound)
            subject.onPrimaryInteract.Invoke();

            Debug.Log("OnTriggerEnter invoke complete");

            if (otherRigidbody != null)
            {

                Debug.Log("OnTriggerEnter in if otherrigidbody section");

                // Calculate the bounce direction (e.g., opposite to the entry direction)
                // For a simple upward bounce:
                Vector3 bounceDirection = Vector3.up;

                // For bouncing relative to the trigger's normal (if it's a plane/wall):
                // You might need to determine the contact point and normal for more precise bounces.
                // For simplicity, let's assume a general upward or outward bounce.

                // Apply an impulse force to the Rigidbody
                otherRigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);

                // Alternatively, directly set the velocity:
                // otherRigidbody.velocity = bounceDirection * bounceForce;

            }
            Debug.Log("OnTriggerEnter - AT END");

            // Return to starting point
            transform.position = Vector3.MoveTowards(transform.position, origin, Time.deltaTime * speed);
            //transform.position = currentPos;
            //Debug.Log("After invoke Position: " + currentPos);
        }
    }
}
