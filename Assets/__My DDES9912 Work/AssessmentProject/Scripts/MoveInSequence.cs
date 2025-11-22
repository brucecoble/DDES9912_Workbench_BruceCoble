using UnityEngine;
using System.Collections;

public class MoveInSequence : MonoBehaviour
{
    public Transform target;      // Assign in Inspector
    public float moveSpeed = 2f;  // Movement speed

    private void Start()
    {
        StartCoroutine(MoveToXYZ());
    }

    IEnumerator MoveToXYZ()
    {
        // Get target position
        Vector3 targetPos = target.position;

        // --- Step 1: Move to Y ---
        yield return StartCoroutine(MoveToPosition(
            new Vector3(transform.position.x, targetPos.y, transform.position.z)));

        // --- Step 2: Move to Z ---
        yield return StartCoroutine(MoveToPosition(
            new Vector3(transform.position.x, targetPos.y, targetPos.z)));

        // --- Step 3: Move to X ---
        yield return StartCoroutine(MoveToPosition(
            new Vector3(targetPos.x, targetPos.y, targetPos.z)));
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
}
