using UnityEngine;
using System.Collections;

public class CapsuleTriggerReturn : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Trigger collider (isTrigger = true) on the target object. Tag it 'Target' or assign a reference.")]
    public Transform target;

    [Header("Motion")]
    public float moveSpeed = 4f;              // units per second
    public float arrivalThreshold = 0.02f;    // how close counts as 'arrived'

    private Vector3 _origin;
    private bool _triggered;

    public InteractableGeneral subject;

    private void Awake()
    {
        _origin = transform.position;
    }

    private IEnumerator Start()
    {
        Debug.Log("Starting CapsuleTriggerReturn");

        // Step 1: move from origin to target
        yield return StartCoroutine(MoveTo(target.position));

        // Step 2: wait for OnTriggerEnter to be raised by physics
        // (OnTriggerEnter sets _triggered = true)
        yield return new WaitUntil(() => _triggered);

        // Optional: reset the flag for next cycles
        _triggered = false;

        // Step 3: move back to origin
        yield return StartCoroutine(MoveTo(_origin));
    }

    private IEnumerator MoveTo(Vector3 destination)
    {
        // Simple constant-speed move until within threshold
        while ((transform.position - destination).sqrMagnitude > (arrivalThreshold * arrivalThreshold))
        {
            Vector3 dir = (destination - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            yield return null;
        }
        // Snap to destination to avoid tiny drift
        transform.position = destination;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Fire only when we hit the intended target
        //if (other.transform == target || other.CompareTag("Target"))
        if (other.transform == target)
        {
            Debug.Log("OnTriggerEnter invoke begun");
            // Do any one-off logic you want at the moment of contact here
            // (e.g., play a VFX/SFX, disable something, etc.)
            // Check if the entering object has a Rigidbody
            Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();

            // Now bounce back
            if (subject != null)
            {

                // Invoke the interaction events (i.e. press the button & make the sound)
                subject.onPrimaryInteract.Invoke();

                Debug.Log("OnTriggerEnter invoke complete");

            }
            _triggered = true;



        }
    }
}
