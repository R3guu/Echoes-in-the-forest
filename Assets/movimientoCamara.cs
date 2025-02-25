using UnityEngine;

public class HandheldCamera : MonoBehaviour
{
    public Transform player; // Referencia al transform del jugador
    public Transform playerCamera; // Referencia a la c�mara principal del jugador
    public Vector3 offset = new Vector3(0.5f, -0.2f, 0.5f); // Posici�n relativa en la mano

    void Update()
    {
        // Mantener la posici�n relativa en la mano del jugador
        transform.position = player.position + player.rotation * offset;

        // Seguir completamente la rotaci�n de la c�mara del jugador
        transform.rotation = playerCamera.rotation;
    }
}
