using UnityEngine;
using UnityEngine.AI;


public class ButtonPusher : MonoBehaviour
{
    public InteractableGeneral subject;
    public GameObject targetGameObject; // Assign your target GameObject in the Inspector

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetGameObject != null)
        {
            // Get the target GameObject's position
            Vector3 targetPosition = targetGameObject.transform.position;

            // Move the current GameObject to the target's position
            // This will instantly teleport the object
            //transform.position = targetPosition;

            // To move smoothly, you can use Vector3.Lerp or Vector3.MoveTowards
            // Example using Lerp for smooth movement:
            // float speed = 5f; // Adjust speed as needed
            // transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);

            // Example using MoveTowards for consistent speed movement:
            float speed = 5f; // Adjust speed as needed
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

        if (subject != null)
        {
            subject.onPrimaryInteract.Invoke();
        }
    }
}
