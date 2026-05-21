using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class DinosaurAI : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float detectionRadius = 10f; // Radius to detect the player
    public float biteRadius = 2f; // Radius within which the dinosaur can bite the player
    public float walkRadius = 5f; // Radius for random walking

    private Animator animator; // Reference to the Animator component
    private NavMeshAgent agent; // Reference to the NavMeshAgent component
    private bool isChasing = false; // Flag to check if the dinosaur is chasing the player
    private bool isIdle = true; // Flag to check if the dinosaur is idle

    private AudioSource roarSource; // AudioSource for roaring
    private AudioSource biteSource; // AudioSource for biting
    public AudioClip roarClip; // Roar audio clip
    public AudioClip biteClip; // Bite audio clip

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // Get the AudioSource components
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            roarSource = audioSources[0];
            biteSource = audioSources[1];
        }

        StartCoroutine(RandomIdleBehavior());
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < biteRadius && isChasing)
        {
            // Bite logic
            BitePlayer();
        }
        else if (distanceToPlayer < detectionRadius)
        {
            // Start chasing the player
            StartChasing();
        }
        else
        {
            // Stop chasing the player
            StopChasing();
        }
    }

    void StartChasing()
    {
        if (!isChasing)
        {
            isChasing = true;
            isIdle = false;
            animator.SetBool("IsChasing", true);
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    void StopChasing()
    {
        if (isChasing)
        {
            isChasing = false;
            animator.SetBool("IsChasing", false);
            agent.isStopped = true;
            StartCoroutine(RandomIdleBehavior());
        }
    }

    void BitePlayer()
    {
        // Play bite sound
        if (biteSource != null && biteClip != null)
        {
            biteSource.PlayOneShot(biteClip);
        }
        Debug.Log("Bite Player!");
    }

    IEnumerator RandomIdleBehavior()
    {
        while (isIdle)
        {
            float waitTime = Random.Range(2f, 5f);
            yield return new WaitForSeconds(waitTime);

            int action = Random.Range(0, 3);
            switch (action)
            {
                case 0:
                    // Roar
                    Roar();
                    break;
                case 1:
                    // Eat
                    animator.SetTrigger("IsEating");
                    break;
                case 2:
                    // Walk
                    Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
                    randomDirection += transform.position;
                    NavMeshHit hit;
                    NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
                    Vector3 finalPosition = hit.position;
                    agent.SetDestination(finalPosition);
                    animator.SetBool("IsWalking", true);
                    agent.isStopped = false;
                    yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);
                    animator.SetBool("IsWalking", false);
                    agent.isStopped = true;
                    break;
            }
        }
    }

    void Roar()
    {
        // Play roar sound
        if (roarSource != null && roarClip != null)
        {
            roarSource.PlayOneShot(roarClip);
        }
        animator.SetTrigger("IsRoaring");
    }
}
