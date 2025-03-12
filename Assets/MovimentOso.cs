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

    void Start()
    {
        ani = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX; // Permitir inclinación
    }

    public void Comportamiento_Enemigo()
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
                rb.MovePosition(rb.position + transform.forward * velocidad * Time.deltaTime);
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