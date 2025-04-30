using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class DeerAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 120f;
    public float walkTimeMin = 3f;
    public float walkTimeMax = 6f;
    public float waitTimeMin = 2f;
    public float waitTimeMax = 4f;
    public float rayDistance = 2f;
    public float roamRadius = 50f;
    public LayerMask obstacleLayers;

    private Rigidbody rb;
    private Animator animator;
    private float stateTimer;
    private bool isWalking;
    private Vector3 homePosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;

        homePosition = transform.position;

        SetNewState();
    }

    void FixedUpdate()
    {
        stateTimer -= Time.fixedDeltaTime;

        if (isWalking)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

            // Verificar obstáculos al frente
            if (Physics.Raycast(rayOrigin, transform.forward, rayDistance, obstacleLayers))
            {
                StopWalking();
                return;
            }

            // Verificar si está a punto de salir del radio permitido
            Vector3 nextPosition = rb.position + transform.forward * moveSpeed * Time.fixedDeltaTime;
            if (Vector3.Distance(homePosition, nextPosition) > roamRadius)
            {
                // Gira hacia una dirección más centrada
                Vector3 toCenter = (homePosition - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(toCenter, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

                StopWalking();
                return;
            }

            // Moverse hacia adelante
            rb.MovePosition(nextPosition);
        }

        // Cambiar estado cuando el tiempo termina
        if (stateTimer <= 0f)
        {
            if (isWalking)
                StopWalking();
            else
                StartWalking();
        }

        animator.SetBool("Walk", isWalking);
    }

    void StartWalking()
    {
        isWalking = true;
        stateTimer = Random.Range(walkTimeMin, walkTimeMax);

        // Girar aleatoriamente
        float randomAngle = Random.Range(-90f, 90f);
        transform.Rotate(0f, randomAngle, 0f);
    }

    void StopWalking()
    {
        isWalking = false;
        stateTimer = Random.Range(waitTimeMin, waitTimeMax);
    }

    void SetNewState()
    {
        if (Random.value > 0.5f)
            StartWalking();
        else
            StopWalking();
    }

    void OnDrawGizmosSelected()
    {
        // Dibujar el radio permitido
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Application.isPlaying ? homePosition : transform.position, roamRadius);

        // Raycast frontal
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawLine(origin, origin + transform.forward * rayDistance);
    }
}
