using UnityEngine;

public class HandheldCamera : MonoBehaviour
{
    public Transform player; // Referencia al transform del jugador
    public Transform playerCamera; // Referencia a la cámara principal del jugador
    public Vector3 offset = new Vector3(0.5f, -0.2f, 0.5f); // Posición relativa en la mano

    void Update()
    {
        // Mantener la posición relativa en la mano del jugador
        transform.position = player.position + player.rotation * offset;

        // Seguir completamente la rotación de la cámara del jugador
        transform.rotation = playerCamera.rotation;
    }
}
