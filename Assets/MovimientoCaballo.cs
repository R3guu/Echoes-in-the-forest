using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentCaballo : MonoBehaviour
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

    void Start()
    {
        ani = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
    }

}  