using UnityEngine;
using UnityEngine.AI;

public class BearAI : MonoBehaviour
{
    public Transform patrolArea; // Un GameObject amb un col·lisionador per definir la zona
    public float patrolRadius = 10f;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        MoveToNewPoint();
    }

    void Update()
    {
        // Si l'ós arriba al punt de destí, buscar un altre lloc on anar
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            MoveToNewPoint();
        }
    }

    void MoveToNewPoint()
    {
        Vector3 randomPoint = GetRandomPointInArea();
        agent.SetDestination(randomPoint);
    }

    Vector3 GetRandomPointInArea()
    {
        Vector3 randomPos = patrolArea.position + new Vector3(
            Random.Range(-patrolRadius, patrolRadius),
            0,
            Random.Range(-patrolRadius, patrolRadius)
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 5f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position; // Si no troba un punt vàlid, es queda on és
    }
}
