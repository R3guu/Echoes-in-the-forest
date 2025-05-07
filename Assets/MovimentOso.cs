using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentOso : MonoBehaviour
{
    public int rutina;
    public float cronometro;
    public Animator ani;
    public Quaternion angulo;
    public float grado;
    public Rigidbody rb;
    public float velocidad = 1f;
    public float velocidadPersecucion = 3f;
    public float distanciaSuelo = 0.5f;

    public Transform jugador;
    public float rangoDeteccion = 10f;
    private bool persiguiendo = false;
    private bool atacando = false;

    private bool esAmigo = false; // NUEVO

    void Start()
    {
        ani = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Comportamiento_Enemigo()
    {
        if (jugador == null || atacando || esAmigo) return; // No hacer nada si es amigo

        float distanciaJugador = Vector3.Distance(transform.position, jugador.position);

        if (distanciaJugador <= rangoDeteccion)
        {
            PersigueJugador();
        }
        else
        {
            persiguiendo = false;
            ani.SetBool("Run Forward", false);
            velocidad = 1f;
            Patrullar();
        }
    }

    void PersigueJugador()
    {
        persiguiendo = true;
        ani.SetBool("WalkForward", false);
        ani.SetBool("Idle", false);
        ani.SetBool("Run Forward", true);
        velocidad = velocidadPersecucion;

        Vector3 direccion = (jugador.position - transform.position).normalized;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out hit, distanciaSuelo + 1f))
        {
            direccion = Vector3.ProjectOnPlane(direccion, hit.normal).normalized;
        }

        transform.rotation = Quaternion.LookRotation(direccion);
        rb.MovePosition(rb.position + direccion * velocidad * Time.deltaTime);
    }

    void Patrullar()
    {
        cronometro += Time.deltaTime;
        if (cronometro >= 4)
        {
            rutina = Random.Range(0, 2);
            cronometro = 0;
        }

        switch (rutina)
        {
            case 0:
                ani.SetBool("WalkForward", false);
                ani.SetBool("Idle", true);
                break;

            case 1:
                grado = Random.Range(0, 360);
                angulo = Quaternion.Euler(0, grado, 0);
                rutina++;
                break;

            case 2:
                transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);

                Vector3 direccion = transform.forward;
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out hit, distanciaSuelo + 1f))
                {
                    direccion = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;
                }

                rb.MovePosition(rb.position + direccion * velocidad * Time.deltaTime);

                ani.SetBool("WalkForward", true);
                ani.SetBool("Idle", false);
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (esAmigo) return; // Si ya es amigo, ignorar colisiones

        if (other.CompareTag("Player") && !atacando)
        {
            StartCoroutine(AtacarJugador(other.gameObject));
        }
    }

    IEnumerator AtacarJugador(GameObject player)
    {
        atacando = true;
        ani.SetBool("Run Forward", false);

        int ataqueAleatorio = Random.Range(1, 9);
        ani.SetTrigger("Attack" + ataqueAleatorio);

        yield return new WaitForSeconds(1.5f);

        if (player != null)
        {
            player.GetComponent<PlayerMovement>().MatarJugador();
        }

        atacando = false;
    }

    void Update()
    {
        if (!esAmigo)
        {
            Comportamiento_Enemigo();
        }
    }

    // NUEVA FUNCIÓN PARA HACERLO AMIGO
    public void HacerseAmigo()
    {
        esAmigo = true;
        ani.SetBool("Run Forward", false);
        ani.SetBool("Idle", true); // O puedes poner una animación amistosa si tienes una
        Debug.Log("¡El oso se ha hecho tu amigo!");
    }
}
