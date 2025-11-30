using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossWalk : MonoBehaviour
{

    [Header("Scene References")]
    public NavMeshAgent theBoss;
    public Transform centerTarget;          // Object in the middle of the room
    public List<Transform> positions;       // Possible positions around the room

    [Header("Animation")]
    public Animator animator;

    // State names in your Animator (Base Layer)
    public string walkingState = "Walking";
    public string stopWalkingState = "StopWalking";
    public string talkingState = "Talking";
    public string stopTalkingState = "StopTalking";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        theBoss = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // ARRIVED?
        if (!theBoss.pathPending &&
            theBoss.remainingDistance <= theBoss.stoppingDistance &&
            theBoss.velocity.sqrMagnitude < 0.01f)   // basically not moving
        {
            
            /*
            if (rotateRoutine == null && centerTarget != null)
            {
                if (arrivalRoutine == null)
                    arrivalRoutine = StartCoroutine(OnArrivedAtDestination());

                if (rotateRoutine == null)
                    rotateRoutine = StartCoroutine(RotateToCenter());
            }
        }
        else
        {
            // MOVING
            //RotateInMoveDirection();
            //PlayWalkingState();
            */
        }
    }




    public void MoveToRandomPosition()
    {
        if (positions == null || positions.Count == 0)
        {
            Debug.LogWarning("No positions assigned on RandomMover.");
            return;
        }

        /*
        // Stop any current arrival / talking behaviour
        if (arrivalRoutine != null)
        {
            StopCoroutine(arrivalRoutine);
            arrivalRoutine = null;
        }

        if (rotateRoutine != null)
        {
            StopCoroutine(rotateRoutine);
            rotateRoutine = null;
        }
        */

        //StopTalkingState();   // optional: cleanly end Talking / audio

        // Pick a random position and set as NavMesh destination
        Transform randomPos = positions[Random.Range(0, positions.Count)];
        theBoss.SetDestination(randomPos.position);

        // Start walking animation immediately
        //PlayWalkingState();
    }


}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossWalk : MonoBehaviour
{
    [Header("Scene References")]
    public Transform centerTarget;          // Object in the middle of the room
    public List<Transform> positions;       // Possible positions around the room

    [Header("Animation")]
    public Animator animator;

    // State names in your Animator (Base Layer)
    public string walkingState = "Walking";
    public string stopWalkingState = "StopWalking";
    public string talkingState = "Talking";
    public string stopTalkingState = "StopTalking";

    // How long to stay in StopWalking before switching to Talking
    public float stopWalkingToTalkingDelay = 0.5f;

    [Header("Rotation")]
    public float rotateToCenterSpeed = 5f;

    [Header("Audio")]
    public AudioSource voiceSource;     // AudioSource with your clip
    public AudioClip voiceClip;         // Clip to play when Talking starts

    private NavMeshAgent agent;
    private Coroutine rotateRoutine;
    private Coroutine arrivalRoutine;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;   // we control rotation manually
    }

    void Update()
    {
        // ARRIVED?
        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            agent.velocity.sqrMagnitude < 0.01f)   // basically not moving
        {
            if (rotateRoutine == null && centerTarget != null)
            {
                if (arrivalRoutine == null)
                    arrivalRoutine = StartCoroutine(OnArrivedAtDestination());

                if (rotateRoutine == null)
                    rotateRoutine = StartCoroutine(RotateToCenter());
            }
        }
        else
        {
            // MOVING
            RotateInMoveDirection();
            PlayWalkingState();
        }
    }


    /// <summary>
    /// Call this to send the character to a random position.
    /// </summary>
    public void MoveToRandomPosition()
    {
        if (positions == null || positions.Count == 0)
        {
            Debug.LogWarning("No positions assigned on RandomMover.");
            return;
        }
        
        // Stop any current arrival / talking behaviour
        if (arrivalRoutine != null)
        {
            StopCoroutine(arrivalRoutine);
            arrivalRoutine = null;
        }

        if (rotateRoutine != null)
        {
            StopCoroutine(rotateRoutine);
            rotateRoutine = null;
        }
        
        StopTalkingState();   // optional: cleanly end Talking / audio

        // Pick a random position and set as NavMesh destination
        Transform randomPos = positions[Random.Range(0, positions.Count)];
        agent.SetDestination(randomPos.position);

        // Start walking animation immediately
        PlayWalkingState();
    }

    // ---------------- ARRIVAL BEHAVIOUR ----------------

    private IEnumerator OnArrivedAtDestination()
    {
        // 1. Play StopWalking, if you have that state
        PlayStopWalkingState();

        // 2. Wait a bit, then go to Talking
        if (stopWalkingToTalkingDelay > 0f)
            yield return new WaitForSeconds(stopWalkingToTalkingDelay);

        PlayTalkingState();
        PlayVoiceAudio();

        arrivalRoutine = null;
    }

    // ---------------- ANIMATION STATE HELPERS ----------------

    private void PlayWalkingState()
    {
        if (animator == null || string.IsNullOrEmpty(walkingState)) return;

        // CrossFade avoids hard popping between states
        animator.CrossFade(walkingState, 0.1f);
    }

    private void PlayStopWalkingState()
    {
        if (animator == null || string.IsNullOrEmpty(stopWalkingState)) return;
        animator.CrossFade(stopWalkingState, 0.1f);
    }

    private void PlayTalkingState()
    {
        if (animator == null || string.IsNullOrEmpty(talkingState)) return;
        animator.CrossFade(talkingState, 0.1f);
    }

    private void StopTalkingState()
    {
        // Optional: go through StopTalking, then maybe Entry/Idle
        if (animator == null) return;

        if (!string.IsNullOrEmpty(stopTalkingState))
        {
            animator.CrossFade(stopTalkingState, 0.1f);
        }

        // Also stop any voice audio
        if (voiceSource != null && voiceSource.isPlaying)
            voiceSource.Stop();
    }

    // ---------------- AUDIO ----------------

    private void PlayVoiceAudio()
    {
        if (voiceSource == null) return;

        if (voiceClip != null)
            voiceSource.clip = voiceClip;

        if (voiceSource.clip != null)
            voiceSource.Play();
    }

    // ---------------- ROTATION HELPERS ----------------

    private void RotateInMoveDirection()
    {
        Vector3 velocity = agent.velocity;
        velocity.y = 0f;

        if (velocity.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotateToCenterSpeed * Time.deltaTime
            );
        }
    }

    private IEnumerator RotateToCenter()
    {
        while (true)
        {
            if (centerTarget == null)
                break;

            Vector3 dir = centerTarget.position - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.0001f)
                break;

            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotateToCenterSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.rotation, targetRot) < 1f)
                break;

            yield return null;
        }

        rotateRoutine = null;
    }
}
*/

/*

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossWalk : MonoBehaviour
{

    public NavMeshAgent myAgent;
    public Transform destination;

    //Header("Chase Settings")] 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        myAgent.destination = destination.position;
    }

}
*/

