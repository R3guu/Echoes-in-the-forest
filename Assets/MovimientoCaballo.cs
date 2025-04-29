using UnityEngine;
using UnityEngine.AI;

public class HorseAI : MonoBehaviour
{
    public float wanderRadius = 20f;
    public float detectionRadius = 3f;
    public float waitTime = 5f;
    public float eatProbability = 0.3f;

    private NavMeshAgent agent;
    private Animator animator;
    private float waitTimer;
    private Vector3 target;

    private enum State { Idle, Moving, Eating }
    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        waitTimer = waitTime;
        currentState = State.Idle;
        ChooseNewTarget();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                animator.SetBool("Moving", true);
                animator.SetBool("Eat", false);

                if (Vector3.Distance(transform.position, target) < 1.5f || IsAnotherHorseTooClose())
                {
                    agent.ResetPath();
                    currentState = State.Idle;
                    waitTimer = Random.Range(3f, waitTime);
                }
                break;

            case State.Idle:
                animator.SetBool("Moving", false);
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0f)
                {
                    if (Random.value < eatProbability)
                    {
                        currentState = State.Eating;
                        animator.SetBool("Eat", true);
                        waitTimer = Random.Range(2f, 5f);
                    }
                    else
                    {
                        ChooseNewTarget();
                        currentState = State.Moving;
                    }
                }
                break;

            case State.Eating:
                animator.SetBool("Moving", false);
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0f)
                {
                    animator.SetBool("Eat", false);
                    currentState = State.Idle;
                    waitTimer = Random.Range(3f, waitTime);
                }
                break;
        }
    }

    void ChooseNewTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            target = hit.position;
            agent.SetDestination(target);
        }
    }

    bool IsAnotherHorseTooClose()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var col in colliders)
        {
            if (col.gameObject != this.gameObject && col.CompareTag("Caballo"))
            {
                return true;
            }
        }
        return false;
    }
}
