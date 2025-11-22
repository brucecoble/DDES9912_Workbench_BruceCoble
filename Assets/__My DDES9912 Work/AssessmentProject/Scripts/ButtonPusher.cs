using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;


public class ButtonPusher : MonoBehaviour
{
    public InteractableGeneral subject;
    public GameObject targetGameObject; // Assign your target GameObject in the Inspector
    private Vector3 currentPos;
    public float speed; // Adjust speed as needed

    // We only want the tip of the pusher to collide - we don't want the pusher to go the the pivot point which is half way
    public float subtractXAmount = 1.15f; // The value to subtract from the X position
    public float subtractYAmount = 0.65f; // The value to subtract from the X position

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get this object's position
        currentPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
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

    private void OnTriggerEnter(Collider other)
    {
        subject = other.GetComponent<InteractableGeneral>();
        Debug.Log("OnTriggerEnter begin");

        if (subject != null)
        {
            // Invoke the interaction events (i.e. press the button & make the sound)
            subject.onPrimaryInteract.Invoke();

            Debug.Log("OnTriggerEnter invoke complete");

            // Return to starting point
            //transform.position = Vector3.MoveTowards(transform.position, currentPos, Time.deltaTime * speed);
            transform.position = currentPos;
            Debug.Log("After invoke Position: " + currentPos);
        }
    }
}
