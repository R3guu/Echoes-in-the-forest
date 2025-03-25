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
    public float velocidadPersecucion = 3f; // Velocidad cuando persigue al jugador
    public float distanciaSuelo = 0.5f;

    public Transform jugador; // Referencia al jugador
    public float rangoDeteccion = 10f; // Rango en el que el oso detecta al jugador
    private bool persiguiendo = false;

    void Start()
    {
        ani = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        jugador = GameObject.FindGameObjectWithTag("Player").transform; // Asegúrate de que el jugador tenga la etiqueta "Player"
    }

    public void Comportamiento_Enemigo()
    {
        if (jugador == null) return;

        float distanciaJugador = Vector3.Distance(transform.position, jugador.position);

        if (distanciaJugador <= rangoDeteccion) // Si el jugador está dentro del área de detección
        {
            PersigueJugador();
        }
        else
        {
            persiguiendo = false;
            ani.SetBool("Run Forward", false);
            velocidad = 1f; // Regresar a la velocidad normal
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

    void Update()
    {
        Comportamiento_Enemigo();
    }
}
