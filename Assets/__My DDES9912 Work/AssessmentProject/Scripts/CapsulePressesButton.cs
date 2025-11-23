using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class CapsulePressesButton : MonoBehaviour
{
    [Header("Refs")]
    public Transform target;              // Trigger collider lives here (IsTrigger = true)

    [Header("Motion")]
    public float moveSpeed = 4f;          // units/sec
    public float arrivalRadius = 0.05f;   // stop when this close to target
    public float triggerTimeout = 3f;     // safety: how long we’ll wait for trigger before giving up

    private Rigidbody rb;
    private Vector3 origin;
    private bool triggered;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false; // we’ll drive it with MovePosition
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Ensure our collider is NOT a trigger
        var col = GetComponent<Collider>();
        col.isTrigger = false;

        origin = transform.position;
    }

    private void OnEnable()
    {
        // kick off the sequence
        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        triggered = false;

        // 1) Move to target
        yield return MoveTo(target.position, arrivalRadius);

        // 2) Wait for OnTriggerEnter (with timeout to avoid hanging if setup is wrong)
        float t = 0f;
        while (!triggered && t < triggerTimeout)
        {
            t += Time.deltaTime;
            yield return null;
        }

        if (!triggered)
        {
            Debug.LogWarning("[CapsuleTriggerReturn] Trigger did not fire. Check collider setup, layers, or 2D/3D mismatch.");
        }

        // 3) Move back to origin
        yield return MoveTo(origin, arrivalRadius);
    }

    // Physics-friendly mover: computes a desired step; actual move happens in FixedUpdate.
    private Vector3 _moveTarget;
    private bool _moving;
    private float _stopRadiusSqr;

    private IEnumerator MoveTo(Vector3 destination, float stopRadius)
    {
        _moveTarget = destination;
        _stopRadiusSqr = stopRadius * stopRadius;
        _moving = true;

        // loop until within radius
        while ((rb.position - _moveTarget).sqrMagnitude > _stopRadiusSqr)
            yield return null;

        // snap to final
        rb.MovePosition(_moveTarget);
        _moving = false;
    }

    void FixedUpdate()
    {
        if (!_moving) return;

        Vector3 to = _moveTarget - rb.position;
        float dist = to.magnitude;
        if (dist <= Mathf.Sqrt(_stopRadiusSqr)) return;

        Vector3 step = to.normalized * moveSpeed * Time.fixedDeltaTime;
        if (step.sqrMagnitude > to.sqrMagnitude) step = to; // don’t overshoot
        rb.MovePosition(rb.position + step);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Fire only when we hit the intended trigger’s collider
        if (other.transform == target)
        {
            triggered = true;
            // Optional: VFX/SFX/etc.
            // Debug.Log("Trigger fired.");
        }
    }
}
