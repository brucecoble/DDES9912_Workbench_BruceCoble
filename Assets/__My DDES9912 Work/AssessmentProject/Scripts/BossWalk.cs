using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossWalk : MonoBehaviour
{
    [Header("Scene References")]
    public NavMeshAgent theBoss;
    public Transform theTypist;
    public List<Transform> positions;

    [Header("Animation")]
    public Animator bossAnimator;
    public Animator typistAnimator;

    // Animator parameter names (bools)
    public string walkBool = "IsWalking";
    public string talkBool = "IsTalking";
    public string pointBool = "IsPointing";

    private Coroutine moveRoutine;

    void Awake()
    {
        if (theBoss == null)
            theBoss = GetComponent<NavMeshAgent>();
    }

    // ---- Public API called from other scripts ----

    public void MoveToRandomPosition()
    {
        if (positions == null || positions.Count == 0 || theBoss == null)
        {
            Debug.LogWarning("BossWalk: positions or NavMeshAgent missing.");
            return;
        }

        Transform target = positions[Random.Range(0, positions.Count)];

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToPositionRoutine(target));
    }

    public void BossTalking()
    {
        // Boss should be standing still and talking
        SetWalking(false);
        SetTalking(true);

        // Typist should be pointing at the same time
        //SetPointing(true);
    }

    public void BossStopTalking()
    {
        SetTalking(false); // will return to Idle via Animator transitions
    }
    public void TypistTyping()
    {
        SetPointing(true);
    }

    public void TypistStopTyping()
    {
        SetPointing(false); // will return to Idle via Animator transitions
    }

    // ---- Internal helpers ----

    private IEnumerator MoveToPositionRoutine(Transform target)
    {
        // Walking: on, Talking: off
        SetTalking(false);
        SetWalking(true);

        FaceTarget(target.position);

        theBoss.isStopped = false;
        theBoss.SetDestination(target.position);

        while (theBoss.pathPending ||
               theBoss.remainingDistance > theBoss.stoppingDistance + 0.05f)
        {
            yield return null;
        }

        theBoss.isStopped = true;

        if (theTypist != null)
        {
            FaceTarget(theTypist.position);
        }

        // Arrived: stop walking → animator will go to Idle
        SetWalking(false);

        moveRoutine = null;
    }

    private void FaceTarget(Vector3 worldPos)
    {
        Vector3 dir = worldPos - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = lookRot;
        }
    }

    private void SetWalking(bool value)
    {
        if (bossAnimator != null && !string.IsNullOrEmpty(walkBool))
        {
            //Debug.Log("SetWalking set value to → " + value.ToString());
            bossAnimator.SetBool(walkBool, value);
        }

        // Ensure Walking and Talking don’t both stay true
        if (value)
            SetTalking(false);
    }

    private void SetTalking(bool value)
    {
        if (bossAnimator != null && !string.IsNullOrEmpty(talkBool))
        {
            //Debug.Log("SetTalking set value to → " + value.ToString());
            bossAnimator.SetBool(talkBool, value);
        }

        // Ensure we don’t walk and talk at the same time unless you want that
        if (value)
            SetWalking(false);
    }

    private void SetPointing(bool value)
    {
        if (typistAnimator != null && !string.IsNullOrEmpty(pointBool))
        {
            //Debug.Log("SetTalking set value to → " + value.ToString());
            typistAnimator.SetBool(pointBool, value);
        }

    }
}
